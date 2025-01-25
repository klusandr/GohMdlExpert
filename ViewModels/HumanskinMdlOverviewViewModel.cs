using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Services;
using GohMdlExpert.ViewModels.Trees.LoadModels;
using GohMdlExpert.ViewModels.Trees.OverviewModels;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public sealed class HumanskinMdlOverviewViewModel : BaseViewModel {
        private MdlFile? _mdlFile;
        private readonly ObservableCollection<PlyModel3D> _plyModels;
        private readonly Model3DCollection _models;
        private readonly ObservableDictionary<string, AggregateMtlFile> _aggregateMtlFiles;
        private readonly Dictionary<string, int> _currentMtlFilesTexturesIndex;
        private readonly Dictionary<PlyModel3D, ObservableCollection<PlyFile>> _lodPlyFiles;
        private readonly CollectionChangeBinder<Model3D> _modelsCollectionBinder;
        private readonly CollectionChangeHandler _plyModelsChangeHandler;

        private readonly IUserDialogProvider _userDialog;
        private readonly GohResourceProvider _resourceProvider;
        private readonly GohTextureProvider _textureProvider;
        private readonly GohHumanskinResourceProvider _humanskinProvider;

        private readonly ModelsOverviewTreeViewModel _modelsOverviewTreeViewModel;
        private readonly PlyModelAdderViewModel _modelAdderViewModel;
        private readonly ModelsLoadTreeViewModel _modelsLoadTreeViewModel;
        private readonly HumanskinMdlGeneratorViewModel _humanskinMdlGeneratorViewModel;
        private readonly DefaultMaterialViewModel _defaultMaterialViewModel;

        public MdlFile? MdlFile {
            get => _mdlFile;
            set {
                _mdlFile = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlyModel3D> PlyModels => _plyModels;
        public ObservableDictionary<string, AggregateMtlFile> AggregateMtlFiles => _aggregateMtlFiles;

        public ModelsOverviewTreeViewModel ModelsOverviewTreeViewModel => _modelsOverviewTreeViewModel;
        public ModelsLoadTreeViewModel ModelsLoadTreeViewModel => _modelsLoadTreeViewModel;
        public PlyModelAdderViewModel ModelAdderViewModel => _modelAdderViewModel;
        public DefaultMaterialViewModel DefaultMaterialViewModel => _defaultMaterialViewModel;

        public Model3DCollection Models => _models;

        public ICommand SaveMdlCommand => CommandManager.GetCommand(SaveMtlFile);
        public ICommand NewMdlCommand => CommandManager.GetCommand(CreateMdlFile);

        public event EventHandler? UpdatedTextures;

        public HumanskinMdlOverviewViewModel(IUserDialogProvider userDialog, GohResourceProvider resourceProvider, GohHumanskinResourceProvider humanskinProvider, GohTextureProvider textureProvider, HumanskinMdlGeneratorViewModel humanskinMdlGeneratorViewModel, MaterialSelector materialSelector) {
            _models = [];
            _plyModels = [];
            _lodPlyFiles = [];
            _aggregateMtlFiles = [];
            _currentMtlFilesTexturesIndex = [];

            _userDialog = userDialog;
            _resourceProvider = resourceProvider;
            _humanskinProvider = humanskinProvider;
            _textureProvider = textureProvider;
            _humanskinMdlGeneratorViewModel = humanskinMdlGeneratorViewModel;

            _defaultMaterialViewModel = new DefaultMaterialViewModel(materialSelector);
            _modelAdderViewModel = new PlyModelAdderViewModel(this, _defaultMaterialViewModel);
            _modelsLoadTreeViewModel = new ModelsLoadTreeViewModel(_modelAdderViewModel, humanskinProvider, textureProvider);
            _modelsOverviewTreeViewModel = new ModelsOverviewTreeViewModel(this);

            _modelsCollectionBinder = new CollectionChangeBinder<Model3D>(_plyModels, _models, (i) => ((PlyModel3D)i!).Model);
            _plyModelsChangeHandler = new CollectionChangeHandler(_plyModels)
                .AddHandlerBuilder(NotifyCollectionChangedAction.Remove, PlyModelRemoveHandler)
                .AddHandlerBuilder(NotifyCollectionChangedAction.Reset, PlyModelRemoveHandler);
        }

        public void CreateMdlFile() {
            MdlFile = new MdlFile("new_humanskin.mdl");
        }

        public void SetMtlFile(MdlFile mdlFile) {
            PlyModels.Clear();
            MdlFile = mdlFile;
            var plyFiles = mdlFile.Data.PlyModel;
            var lodFiles = mdlFile.Data.PlyModelLods;
            var mtlFiles = new List<MtlFile>();


            foreach (var plyFile in plyFiles) {
                _humanskinProvider.Current.SetPlyFileFullPath(plyFile);

                string? mdlFilePath = mdlFile.GetDirectoryPath();

                if (mdlFilePath != null) {
                    foreach (var mtlFilePath in Directory.GetFiles(mdlFilePath, "*.mtl")) {
                        mtlFiles.Add(new MtlFile(mtlFilePath));
                    }
                }
            }

            var missTexture = new List<MtlFile>();

            _textureProvider.SetTexturesMaterialsFullPath(mtlFiles.Select(m => m.Data));

            foreach (var item in mdlFile.Data.PlyModel) {
                item.RelativePathPoint = _humanskinProvider.Current.Source.Path;
            }

            foreach (var item in mdlFile.Data.PlyModelLods.Values.SelectMany(p => p)) {
                item.RelativePathPoint = _humanskinProvider.Current.Source.Path;
            }

            foreach (var plyFile in plyFiles) {
                AddModel(new PlyModel3D(plyFile), lodModels: lodFiles[plyFile]);
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

        public void AddModel(PlyModel3D modelPly, AggregateMtlFiles? aggregateMtlFiles = null, IEnumerable<PlyFile>? lodModels = null) {
            if (MdlFile == null) {
                CreateMdlFile();
            }

            if (aggregateMtlFiles != null) {
                foreach (var aggregateMtlFile in aggregateMtlFiles) {
                    AddAggregateMtlFile(aggregateMtlFile);
                }
            }

            _lodPlyFiles.Add(modelPly, new ObservableCollection<PlyFile>(lodModels ?? ResourceLoading.GetPlyLodFiles(modelPly.PlyFile, _humanskinProvider.Current, _resourceProvider)));

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

        public void RemoveAggregateMtlFile(string aggregateMtlFileName) {
            _aggregateMtlFiles.Remove(aggregateMtlFileName);
            _currentMtlFilesTexturesIndex.Remove(aggregateMtlFileName);
            UpdateTexture();
        }

        public void ClearAggregateFiles() {
            _aggregateMtlFiles.Clear();
            _currentMtlFilesTexturesIndex.Clear();
            UpdateTexture();
        }

        public void SaveMtlFile() {
            if (MdlFile == null) {
                return;
            }

            _humanskinMdlGeneratorViewModel.CreateMtlFile(
                MdlFile,
                PlyModels.Select(p => p.PlyFile),
                new Dictionary<string, MtlTexture>(AggregateMtlFiles.Values.Select(m => new KeyValuePair<string, MtlTexture>(m.Name, m.Data.ElementAt(GetMtlFileMaterialIndex(m.Name))))),
                new Dictionary<PlyFile, PlyFile[]>(_lodPlyFiles.Select(l => new KeyValuePair<PlyFile, PlyFile[]>(l.Key.PlyFile, l.Value.ToArray())))
            );
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

        public ObservableCollection<PlyFile> GetPlyModelLodFiles(PlyModel3D modelPly) {
            if (!_lodPlyFiles.TryGetValue(modelPly, out var value)) {
                throw new Exception("Модель говно");
            } else {
                return value;
            }
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
                        RemoveAggregateMtlFile(aggregateMtlFile.Name);
                    }
                }
            }
        }
    }
}
