using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public sealed class HumanskinMdlOverviewViewModel : BaseViewModel {
        private readonly ObservableCollection<PlyModel3D> _plyModels;
        private readonly Model3DCollection _models;
        private readonly ObservableDictionary<string, AggregateMtlFile> _aggregateMtlFiles;
        private readonly Dictionary<string, int> _currentMtlFilesTexturesIndex;
        private readonly Dictionary<PlyModel3D, IEnumerable<PlyFile>> _lodPlyFiles;
        private readonly CollectionChangeBinder<Model3D> _modelsCollectionBinder;
        private readonly CollectionChangeHandler _plyModelsChangeHandler;
        private MdlFile? _mdlFile;
        private PlyModel3D? _addedModel;

        private readonly IUserDialogProvider _userDialog;
        private readonly GohTextureProvider _gohTextureProvider;
        private readonly GohHumanskinResourceProvider _humanskinProvider;

        private readonly ModelsOverviewTreeViewModel _modelsOverviewTreeViewModel;

        public MdlFile? MdlFile {
            get => _mdlFile;
            set {
                _mdlFile = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlyModel3D> PlyModels => _plyModels;
        public ObservableDictionary<string, AggregateMtlFile> AggregateMtlFiles => _aggregateMtlFiles;

        public PlyModel3D? AddedModel {
            get => _addedModel;
            set {
                _addedModel = value;
                OnPropertyChanged();
            }
        }

        public ModelsOverviewTreeViewModel ModelsOverviewTreeViewModel => _modelsOverviewTreeViewModel;

        public Model3DCollection Models => _models;

        public event EventHandler? UpdatedTextures;

        public HumanskinMdlOverviewViewModel(IUserDialogProvider userDialog, GohTextureProvider gohTextureProvider, GohHumanskinResourceProvider humanskinProvider) {
            _models = [];
            _plyModels = [];
            _lodPlyFiles = [];
            _aggregateMtlFiles = [];
            _currentMtlFilesTexturesIndex = [];

            _userDialog = userDialog;
            _gohTextureProvider = gohTextureProvider;
            _humanskinProvider = humanskinProvider;

            _modelsOverviewTreeViewModel = new ModelsOverviewTreeViewModel(this);

            _modelsCollectionBinder = new CollectionChangeBinder<Model3D>(_plyModels, _models, (i) => ((PlyModel3D)i!).Model);
            _plyModelsChangeHandler = new CollectionChangeHandler(_plyModels)
                .AddHandlerBuilder(NotifyCollectionChangedAction.Remove, PlyModelRemoveHandler)
                .AddHandlerBuilder(NotifyCollectionChangedAction.Reset, PlyModelRemoveHandler);
        }

        public void SetMtlFile(MdlFile mdlFile) {
            PlyModels.Clear();
            MdlFile = mdlFile;
            var plyFiles = ResourceLoading.FilterPlyFiles(mdlFile.Data.PlyModelFiles);
            var mtlFiles = mdlFile.Data.Textures;
            var missTexture = new List<MtlFile>();

            _gohTextureProvider.SetTexturesMaterialsFullPath(mtlFiles.Select(m => m.Data));

            foreach (var plyFile in plyFiles) {
                AddModel(new PlyModel3D(plyFile));
            }

            foreach (var mtlFile in mtlFiles) {
                var plyFile = plyFiles.FirstOrDefault(p => p.Data.Meshes.Select(m => m.TextureName).Contains(mtlFile.Name));

                if (plyFile != null) {
                    AddAggregateMtlFile(new AggregateMtlFile(mtlFile, plyFile));
                } else {
                    missTexture.Add(mtlFile);
                }
            }

            UpdateTexture();

            if (missTexture.Count != 0) {
                _userDialog.ShowWarning($"Loaded .mdl file {mdlFile.GetFullPath()}, contains unclaimed textures: [{string.Join(", ", missTexture.Select(m => m.Name))}] that have been removed.", "Miss textures");
            }
        }

        public void AddModel(PlyModel3D modelPly, AggregateMtlFiles? aggregateMtlFiles = null) {
            MdlFile ??= CreateMdlFile();

            if (aggregateMtlFiles != null) {
                foreach (var aggregateMtlFile in aggregateMtlFiles) {
                    AddAggregateMtlFile(aggregateMtlFile);
                }
            }

            _lodPlyFiles.Add(modelPly, _humanskinProvider.Current!.GetPlyLodFiles(modelPly.PlyFile));

            _plyModels.Add(modelPly);
            UpdateTexture();
        }

        public void RemoveModel(PlyModel3D modelPly) {
            _plyModels.Remove(modelPly);
        }

        public void ClearModels() {
            _plyModels.Clear();
        }

        public void AddAggregateMtlFile(AggregateMtlFile aggregateMtlFile) {
            if (_aggregateMtlFiles.TryGetValue(aggregateMtlFile.Name, out var usedAggregateFile)) {
                MergeAggregateTextures(usedAggregateFile, aggregateMtlFile);
            } else {
                _currentMtlFilesTexturesIndex.Add(aggregateMtlFile.Name, 0);
                _aggregateMtlFiles.Add(aggregateMtlFile.Name, aggregateMtlFile);
            }
        }

        public void RemoveAggregateFile(string aggregateMtlFileName) {
            _aggregateMtlFiles.Remove(aggregateMtlFileName);
            _currentMtlFilesTexturesIndex.Remove(aggregateMtlFileName);
            UpdateTexture();
        }

        public void ClearAggregateFiles() {
            _aggregateMtlFiles.Clear();
            _currentMtlFilesTexturesIndex.Clear();
            UpdateTexture();
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

        public IEnumerable<PlyFile> GetPlyModelLodFiles(PlyModel3D modelPly) {
            if (!_lodPlyFiles.TryGetValue(modelPly, out var value)) {
                throw new Exception("Модель говно");
            } else {
                return value;
            }
        }

        private MdlFile? CreateMdlFile() {
            return new MdlFile("new_humanskin.mdl");
        }

        private void MergeAggregateTextures(AggregateMtlFile oldMtlFile, AggregateMtlFile newMtlFile) {
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
                    string.Format("Added texture \"{0}\" don't contains: [{1}] materials which are contains in used textures." +
                        "Delete uncontained materials in used textures.",
                        newMtlFile.Name,
                        string.Join(", ", differenceMaterials.Select(t => t.Diffuse.Name))),
                    "Texture combined", QuestionType.YesNoCancel);

                if (result == QuestionResult.Cancel) {
                    throw new OperationCanceledException();
                } else if (result == QuestionResult.Yes) {
                    oldMtlFile.Data = new MtlTextureCollection(oldMtlFile.Data.Intersect(newMtlFile.Data));
                    _currentMtlFilesTexturesIndex[oldMtlFile.Name] = 0;
                }
            }
        }

        private void UpdateTexture() {
            foreach (var model in _plyModels) {
                foreach (var meshTextureName in model.MeshesTextureNames) {
                    if (_aggregateMtlFiles.ContainsKey(meshTextureName)) {
                        model.SetMeshTexture(meshTextureName, GetCurrentMtlFileTexture(meshTextureName));
                    } else {
                        model.SetMeshTexture(meshTextureName, null);
                    }
                }
            }

            UpdatedTextures?.Invoke(this, EventArgs.Empty);
        }

        private void PlyModelRemoveHandler(object? sender, NotifyCollectionChangedEventArgs e) {
            if (_plyModels.Count == 0) {
                ClearAggregateFiles();
                
            } else {
                _lodPlyFiles.Remove(e.GetItem<PlyModel3D>()!);

                foreach (var aggregateMtlFile in _aggregateMtlFiles.Values) {
                    if (!GetMtlFilePlyModels(aggregateMtlFile.Name).Any()) {
                        RemoveAggregateFile(aggregateMtlFile.Name);
                    }
                }
            }
        }
    }
}
