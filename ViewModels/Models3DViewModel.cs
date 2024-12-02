using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public sealed class Models3DViewModel : BaseViewModel {
        private readonly ObservableCollection<PlyModel3D> _plyModels;
        private readonly ObservableDictionary<string, PlyAggregateMtlFile> _aggregateMtlFiles;
        private readonly ObservableDictionary<string, int> _currentMtlFilesTexturesIndex;
        private readonly Model3DCollection _models;
        private PlyModel3D? _addedModel;

        private readonly IUserDialogProvider _userDialog;

        public ObservableCollection<PlyModel3D> PlyModels => _plyModels;
        public ObservableDictionary<string, PlyAggregateMtlFile> AggregateMtlFiles => _aggregateMtlFiles;
        public Model3DCollection Models => _models;

        public PlyModel3D? AddedModel {
            get => _addedModel;
            set {
                _addedModel = value;
                OnPropertyChanged();
            }
        }

        public Point3D? ModelsCenter => ((GeometryModel3D?)Models.FirstOrDefault())?.GetCentrPoint();

        public Models3DViewModel(IUserDialogProvider userDialog) {
            _models = [];
            _plyModels = [];
            _aggregateMtlFiles = [];
            _currentMtlFilesTexturesIndex = [];

            _plyModels.CollectionChanged += PlyModelsChanged;
            _userDialog = userDialog;
        }

        public void AddModel(PlyModel3D modelPly, PlyAggregateMtlFiles? aggregateMtlFiles) {
            _plyModels.Add(modelPly);

            if (aggregateMtlFiles != null) {
                AddAggregateMtlFiles(aggregateMtlFiles);
            }
        }

        public void AddAggregateMtlFiles(PlyAggregateMtlFiles aggregateMtlFiles) {
            foreach (var newAggregateFile in aggregateMtlFiles) {
                if (_aggregateMtlFiles.TryGetValue(newAggregateFile.Name, out var usedAggregateFile)) {
                    var equalsResult = usedAggregateFile.Data.Equals(newAggregateFile.Data);

                    switch (equalsResult) {
                        case MtlTextureCollection.CollectionEquality.No:
                            throw TextureException.AggregateFilesInconsistency(newAggregateFile, usedAggregateFile);
                        case MtlTextureCollection.CollectionEquality.Partial:
                            var result = _userDialog.Ask(
                                string.Format("Added texture \"{0}\" partial inconsistency with used textures \"{1}\". " +
                                    "Combine them? When combined, some materials will become unavailable.",
                                    newAggregateFile,
                                    usedAggregateFile),
                                "Texture combined");

                            if (result == QuestionResult.OK) {
                                MargeAggregateTexture(usedAggregateFile, newAggregateFile);
                            }
                            break;
                        case MtlTextureCollection.CollectionEquality.Full:
                            break;
                    }
                } else {
                    _currentMtlFilesTexturesIndex.Add(newAggregateFile.Name, 0);
                    _aggregateMtlFiles.Add(newAggregateFile.Name, newAggregateFile);
                }
            }
        }

        public void RemoveModel(PlyModel3D modelPly) {
            _plyModels.Remove(modelPly);
        }

        public void Clear() {
            _plyModels.Clear();
        }

        public MtlTexture GetCurrentMtlFileTexture(string meshTextureName) {
            return GetMtlFileTextureByIndex(meshTextureName, _currentMtlFilesTexturesIndex[meshTextureName]);
        }

        public void SetMtlFileTextureByIndex(string meshTextureName, int index) {
            if (_aggregateMtlFiles.TryGetValue(meshTextureName, out var aggregateTextures)) {
                var mtlTexture = aggregateTextures.Data.ElementAt(index);
                _currentMtlFilesTexturesIndex[meshTextureName] = index;

                foreach (var model in _plyModels.Where(m => m.MeshesNames.Contains(meshTextureName))) {
                    model.SetMeshTexture(meshTextureName, mtlTexture);
                }
            }
        }

        public MtlTexture GetMtlFileTextureByIndex(string meshTextureName, int index) {
            if (_aggregateMtlFiles.TryGetValue(meshTextureName, out var aggregateTextures)) {
                if (index >= 0 && index < aggregateTextures.Data.Count) {
                    return aggregateTextures.Data.ElementAt(index);
                } else {
                    throw new Exception("Индекс говно");
                }
            } else {
                throw new Exception("Имя говно");
            }
        }

        private Point3D GetPointsCenter(params Point3D[] points3D) {
            return new Point3D() {
                X = points3D.Average(p => p.X),
                Y = points3D.Average(p => p.Y),
                Z = points3D.Average(p => p.Z),
            };
        }

        private void MargeAggregateTexture(PlyAggregateMtlFile oldAggregateTexture, PlyAggregateMtlFile newAggregateTexture) {
            oldAggregateTexture.Data = new MtlTextureCollection(oldAggregateTexture.Data.Intersect(newAggregateTexture.Data, MtlTexture.GetEqualityComparer()));
            UpdateTexture();
        }

        private void UpdateTexture() {
            List<(MtlTexture texture, string name, PlyFile? plyFile)> miss = [];

            foreach (var aggregateTextures in _aggregateMtlFiles) {
                string meshTextureName = aggregateTextures.Value.Name;
                int mtlFileTextureIndex = _currentMtlFilesTexturesIndex[meshTextureName];

                foreach (var model in _plyModels.Where(m => m.MeshesNames.Contains(meshTextureName))) {
                    var texture = model.GetMeshTexture(meshTextureName);

                    if (texture != null && !aggregateTextures.Value.Data.Contains(texture)) {
                        model.SetMeshTexture(meshTextureName, aggregateTextures.Value.Data.ElementAt(mtlFileTextureIndex));
                        miss.Add((texture, meshTextureName, model.PlyFile));
                    }
                }
            }

            if (miss.Count != 0) {
                _userDialog.ShowWarning($"{miss.Count} models reset.");
            }
        }

        private void PlyModelsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems!.OfType<PlyModel3D>()) {
                        _models.Add(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.NewItems!.OfType<PlyModel3D>()) {
                        _models.Add(oldItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _models[e.NewStartingIndex] = (PlyModel3D?)e.NewItems?[0];
                    break;
                case NotifyCollectionChangedAction.Move:
                    var item = (PlyModel3D?)e.NewItems?[0];
                    _models.Remove(item);
                    _models.Insert(e.NewStartingIndex, item);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _models.Clear();
                    break;
            }
        }

    }
}
