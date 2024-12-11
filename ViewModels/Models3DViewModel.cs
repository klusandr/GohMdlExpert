using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
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
        private readonly Dictionary<string, int> _currentMtlFilesTexturesIndex;
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

        public event EventHandler? UpdatedTextures;

        public Models3DViewModel(IUserDialogProvider userDialog) {
            _models = [];
            _plyModels = [];
            _aggregateMtlFiles = [];
            _currentMtlFilesTexturesIndex = [];

            _plyModels.CollectionChanged += PlyModelsChanged;
            _userDialog = userDialog;
        }

        public void AddModel(PlyModel3D modelPly, PlyAggregateMtlFiles? aggregateMtlFiles) {
            if (aggregateMtlFiles != null) {
                foreach (var aggregateMtlFile in aggregateMtlFiles) {
                    AddAggregateMtlFile(aggregateMtlFile);
                }
            }

            _plyModels.Add(modelPly);
            UpdateTexture();
        }

        public void RemoveModel(PlyModel3D modelPly) {
            _plyModels.Remove(modelPly);
        }

        public void Clear() {
            _plyModels.Clear();
        }

        public void AddAggregateMtlFile(PlyAggregateMtlFile aggregateMtlFile) {
            if (_aggregateMtlFiles.TryGetValue(aggregateMtlFile.Name, out var usedAggregateFile)) {
                MergeAggregateTextures(usedAggregateFile, aggregateMtlFile);
            } else {
                _currentMtlFilesTexturesIndex.Add(aggregateMtlFile.Name, 0);
                _aggregateMtlFiles.Add(aggregateMtlFile.Name, aggregateMtlFile);
            }
        }

        public void SetMtlFileTextureByIndex(string meshTextureName, int index) {
            if (_aggregateMtlFiles.TryGetValue(meshTextureName, out var aggregateTextures)) {
                if (index >= 0 && index < aggregateTextures.Data.Count) {
                    var mtlTexture = aggregateTextures.Data.ElementAt(index);
                    _currentMtlFilesTexturesIndex[meshTextureName] = index;

                    foreach (var model in GetMtlFilePlyModels(meshTextureName)) {
                        model.SetMeshTexture(meshTextureName, mtlTexture);
                    }

                    UpdatedTextures?.Invoke(this, EventArgs.Empty);
                } else {
                    throw new Exception("Индекс говно");
                }
            } else {
                throw new Exception("Имя говно");
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

        public MtlTexture GetCurrentMtlFileTexture(string meshTextureName) {
            return GetMtlFileTextureByIndex(meshTextureName, GetMtlFileMaterialIndex(meshTextureName));
        }

        public int GetMtlFileMaterialIndex(string meshTextureName) {
            if (_currentMtlFilesTexturesIndex.TryGetValue(meshTextureName, out var index)) {
                return index;
            } else {
                throw new Exception("Имя говно");
            }
        }

        public IEnumerable<PlyModel3D> GetMtlFilePlyModels(string mtlFileName) {
            return _plyModels.Where(p => p.MeshesTextureNames.Contains(mtlFileName));
        }

        private void MergeAggregateTextures(PlyAggregateMtlFile oldMtlFile, PlyAggregateMtlFile newMtlFile) {
            var differenceMaterials = oldMtlFile.Data.Where(t => !newMtlFile.Data.Contains(t));

            if (differenceMaterials.Count() == oldMtlFile.Data.Count) {
                var result = _userDialog.Ask(
                    string.Format("Added texture \"{0}\" that which is not compatible with used textures. Should I add it anyway? This may cause incorrect texture mapping.", newMtlFile.Name),
                    "Tot compatible texture");
                if (result == QuestionResult.Cancel) {
                    throw new OperationCanceledException();
                }
            } else if (differenceMaterials.Any()) {
                var result = _userDialog.Ask(
                    string.Format("Added texture \"{0}\" don't contains \"{1}\" materials which are contains in used textures." +
                        "Delete uncontained materials in used textures.",
                        newMtlFile.Name,
                        string.Join("; ", differenceMaterials.Select(t => t.Diffuse.Name))),
                    "Texture combined", QuestionType.YesNoCancel);

                if (result == QuestionResult.Cancel) {
                    throw new OperationCanceledException();
                } else if (result == QuestionResult.Yes) {
                    oldMtlFile.Data = new MtlTextureCollection(oldMtlFile.Data.Intersect(newMtlFile.Data));
                }
            }
        }

        private Point3D GetPointsCenter(params Point3D[] points3D) {
            return new Point3D() {
                X = points3D.Average(p => p.X),
                Y = points3D.Average(p => p.Y),
                Z = points3D.Average(p => p.Z),
            };
        }

        private void UpdateTexture() {
            foreach (var model in _plyModels) {
                foreach (var meshTextureName in model.MeshesTextureNames) {
                    if (_aggregateMtlFiles.ContainsKey(meshTextureName)) {
                        model.SetMeshTexture(meshTextureName, GetCurrentMtlFileTexture(meshTextureName));
                    }
                }
            }

            UpdatedTextures?.Invoke(this, EventArgs.Empty);
        }

        private void PlyModelsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems!.OfType<PlyModel3D>()) {
                        _models.Add(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems!.OfType<PlyModel3D>()) {
                        _models.Remove(oldItem);
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
