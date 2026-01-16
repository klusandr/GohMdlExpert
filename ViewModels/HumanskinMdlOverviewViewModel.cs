using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Services;
using GohMdlExpert.ViewModels.Trees.Humanskins;
using GohMdlExpert.ViewModels.Trees.LoadModels;
using GohMdlExpert.ViewModels.Trees.OverviewModels;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;
using WpfMvvm.ViewModels;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.ViewModels {
    public sealed class HumanskinMdlOverviewViewModel : BaseViewModel {
        private MdlFile? _mdlFile;
        private PlyModel3D? _focusablePlyModel;
        private IEnumerable<int>? _humanskinLodLevels;
        private int _humanskinLodLevel;
        private readonly ObservableCollection<PlyModel3D> _plyModels;
        private readonly Model3DCollection _models;
        private readonly ObservableDictionary<string, AggregateMtlFile> _aggregateMtlFiles;
        private readonly Dictionary<string, int> _currentMtlFilesTexturesIndex;
        private readonly CollectionChangeBinder<Model3D> _modelsCollectionBinder;
        private readonly CollectionChangeHandler _plyModelsChangeHandler;

        private readonly IUserDialogProvider _userDialog;
        private readonly GohResourceProvider _resourceProvider;
        private readonly GohTextureProvider _textureProvider;
        private readonly GohHumanskinResourceProvider _humanskinProvider;

        private readonly ModelsOverviewTreeViewModel _modelsOverviewTreeViewModel;
        private readonly PlyModelAdderViewModel _modelAdderViewModel;
        private readonly ModelsLoadTreeViewModel _modelsLoadTreeViewModel;
        private readonly HumanskinTreeViewModel _humanskinTreeViewModel;
        private readonly HumanskinSaveService _humanskinSeveSrvice;
        private readonly DefaultTextureViewModel _defaultMaterialViewModel;

        public MdlFile? MdlFile {
            get => _mdlFile;
            set {
                if (_mdlFile != value) {
                    _mdlFile?.UnloadData();
                }

                _mdlFile = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlyModel3D> PlyModels => _plyModels;
        public ObservableDictionary<string, AggregateMtlFile> AggregateMtlFiles => _aggregateMtlFiles;
        public Model3DCollection Models => _models;

        public bool Autofocus { get; set; } = true;

        public PlyModel3D? FocusablePlyModel {
            get => _focusablePlyModel;
            set {
                _focusablePlyModel = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<int>? HumanskinLodLevels {
            get => _humanskinLodLevels;
            set {
                _humanskinLodLevels = value;
                OnPropertyChanged();
            }
        }

        public int HumanskinLodLevel {
            get => _humanskinLodLevel;
            set {
                _humanskinLodLevel = value;

                foreach (var plyModel in _plyModels) {
                    plyModel.SetLodIndex(value);
                }

                OnPropertyChanged();
            }
        }

        public ModelsOverviewTreeViewModel ModelsOverviewTreeViewModel => _modelsOverviewTreeViewModel;
        public ModelsLoadTreeViewModel ModelsLoadTreeViewModel => _modelsLoadTreeViewModel;
        public HumanskinTreeViewModel HumanskinTreeViewModel => _humanskinTreeViewModel;
        public PlyModelAdderViewModel ModelAdderViewModel => _modelAdderViewModel;
        public DefaultTextureViewModel DefaultMaterialViewModel => _defaultMaterialViewModel;

        public ICommand SaveMdlCommand => CommandManager.GetCommand(SaveMtlFile, canExecute: (_) => MdlFile != null);
        public ICommand NewMdlCommand => CommandManager.GetCommand(CreateMdlFile);
        public ICommand ClearPlyModelFocusCommand => CommandManager.GetCommand(ClearPlyModelFocus);

        public event EventHandler? UpdatedTextures;

        public HumanskinMdlOverviewViewModel(IUserDialogProvider userDialog, GohResourceProvider resourceProvider, GohHumanskinResourceProvider humanskinProvider, GohTextureProvider textureProvider, HumanskinSaveService humanskinSeveSrvice, TextureLoadService textureSelector, SelectResourceFileService selectResourceFileService) {
            _models = [];
            _plyModels = [];
            _aggregateMtlFiles = [];
            _currentMtlFilesTexturesIndex = [];

            _userDialog = userDialog;
            _resourceProvider = resourceProvider;
            _humanskinProvider = humanskinProvider;
            _textureProvider = textureProvider;
            _humanskinSeveSrvice = humanskinSeveSrvice;

            _defaultMaterialViewModel = new DefaultTextureViewModel(textureSelector);
            _modelAdderViewModel = new PlyModelAdderViewModel(this, _defaultMaterialViewModel);
            _modelsLoadTreeViewModel = new ModelsLoadTreeViewModel(_modelAdderViewModel, _defaultMaterialViewModel, humanskinProvider, textureProvider);
            _humanskinTreeViewModel = new HumanskinTreeViewModel(this, humanskinProvider);
            _modelsOverviewTreeViewModel = new ModelsOverviewTreeViewModel(this, textureSelector, selectResourceFileService);

            PropertyChangeHandler.AddHandler(nameof(MdlFile), (_, _) => CommandManager.OnCommandCanExecuteChanged(nameof(SaveMdlCommand)));
            _modelsCollectionBinder = new CollectionChangeBinder<Model3D>(_plyModels, _models,
                (s) => {
                    var plyModel = (s as PlyModel3D)!;
                    plyModel.ModelChanged += PlyModelChanged;
                    return plyModel.Model;
                });
            _plyModelsChangeHandler = new CollectionChangeHandler(_plyModels)
                .AddHandlerBuilder(NotifyCollectionChangedAction.Remove, PlyModelRemoveHandler)
                .AddHandlerBuilder(NotifyCollectionChangedAction.Reset, PlyModelRemoveHandler);
        }

        public void CreateMdlFile() {
            PlyModels.Clear();
            MdlFile = new MdlFile("new_humanskin.mdl");
        }

        public void SetMtlFile(MdlFile mdlFile) {
            PlyModels.Clear();

            _humanskinProvider.Resource.MdlMdoelInitialize(mdlFile.Data);
            GohResourceLoading.LoadHumanskinFile(mdlFile, out var mtlFiles, _resourceProvider, _textureProvider);

            MdlFile = mdlFile;
            var plyFiles = mdlFile.Data.PlyModels;
            var lodFiles = mdlFile.Data.PlyModelsLods;

            var missTexture = new List<MtlFile>();

            foreach (var mtlFile in mtlFiles) {
                var plyFile = plyFiles.FirstOrDefault(p => p.Data.Meshes.Select(m => m.TextureName).Contains(mtlFile.Name));

                if (plyFile != null) {
                    AddAggregateMtlFile(new AggregateMtlFile(mtlFile));
                } else {
                    missTexture.Add(mtlFile);
                }
            }

            foreach (var plyFile in plyFiles) {
                lodFiles.TryGetValue(plyFile, out var lodFile);
                AddModel(new PlyModel3D(plyFile, lodPlyFiles: lodFile));
            }

            UpdateTextures();

            if (missTexture.Count != 0) {
                _userDialog.ShowWarning($"Loaded .mdl file {mdlFile.GetFullPath()}, contains unclaimed textures: [{string.Join(", ", missTexture.Select(m => m.Name))}] that have been removed.", "Miss textures");
            }

            ClearPlyModelFocus();
        }

        public void AddModel(PlyModel3D modelPly, AggregateMtlFiles? aggregateMtlFiles = null) {
            if (MdlFile == null) {
                CreateMdlFile();
            }

            if (aggregateMtlFiles != null) {
                foreach (var aggregateMtlFile in aggregateMtlFiles) {
                    AddAggregateMtlFile(aggregateMtlFile);
                }
            }

            if (modelPly.LodPlyFiles.Count == 0) {
                var result = _userDialog.Ask("Ply model don't have LOD models. Add null LOD?", "LOD models", QuestionType.YesNoCancel);

                if (result == QuestionResult.Yes) {
                    var nullLod = _humanskinProvider.Resource.GetNullPlyFile(modelPly.PlyFile);

                    if (nullLod != null) {
                        modelPly.LodPlyFiles.Add(nullLod);
                    } else {
                        _userDialog.ShowWarning(string.Format("Plgramm don't find null LOD moder for \"{0}\".", modelPly.PlyFile.GetFullPath()), "LOD models");
                    }
                } else if (result == QuestionResult.Cancel) {
                    throw new OperationCanceledException();
                }
            }

            _plyModels.Add(modelPly);
            UpdateTextures();
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
            UpdateTextures();
        }

        public void ClearAggregateFiles() {
            _aggregateMtlFiles.Clear();
            _currentMtlFilesTexturesIndex.Clear();
            UpdateTextures();
        }

        public void SaveMtlFile() {
            if (MdlFile == null) {
                return;
            }

            MdlModel? mdlModel = null;

            MdlFile.Data = new MdlModel(
                mdlModel?.Parameters ?? GohResourceLoading.MdlTemplateParameters,
                PlyModels.Select(p => p.PlyFile),
                new(PlyModels.Select(l => new KeyValuePair<PlyFile, PlyFile[]>(l.PlyFile, [.. l.LodPlyFiles])))
            );

            var textures = new Dictionary<string, MtlTexture>(
                AggregateMtlFiles.Keys.Select(
                    (n) => new KeyValuePair<string, MtlTexture>(n, GetCurrentMtlFileTexture(n) ?? throw GohResourceSaveException.MtlTextureIsNotDefindForMesh(n))
                )
            );

            _humanskinSeveSrvice.Save(MdlFile, textures);
            OnPropertyChanged(nameof(MdlFile));
        }

        public void SetMtlFileTextureByIndex(string meshTextureName, int index) {
            if (_aggregateMtlFiles.TryGetValue(meshTextureName, out var aggregateTextures)) {
                if (aggregateTextures.Data.Count == 0) {
                    return;
                }

                if (index >= 0 && index < aggregateTextures.Data.Count) {
                    var mtlTexture = aggregateTextures.Data.ElementAt(index);
                    _currentMtlFilesTexturesIndex[meshTextureName] = index;

                    foreach (var model in GetMtlFilePlyModels(meshTextureName)) {
                        model.SetMeshTexture(meshTextureName, mtlTexture);
                    }

                    UpdatedTextures?.Invoke(this, EventArgs.Empty);
                } else if (index == -1) {
                    foreach (var model in GetMtlFilePlyModels(meshTextureName)) {
                        model.SetMeshTexture(meshTextureName, MtlTexture.NullTexture);
                    }
                } else {
                    throw new IndexOutOfRangeException(string.Format("Humanskin model .mtl aggregate textures do not contain element with index: {0}.", index));
                }
            } else {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }
        }

        public MtlTexture? GetMtlFileTextureByIndex(string meshTextureName, int index) {
            if (_aggregateMtlFiles.TryGetValue(meshTextureName, out var aggregateTextures)) {
                if (aggregateTextures.Data.Count == 0) {
                    return null;
                }

                if (index >= 0 && index < aggregateTextures.Data.Count) {
                    return aggregateTextures.Data.ElementAt(index);
                } else {
                    throw new IndexOutOfRangeException(string.Format("Humanskin model .mtl aggregate textures do not contain element with index: {0}.", index));
                }
            } else {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }
        }

        public MtlTexture? GetCurrentMtlFileTexture(string meshTextureName) {
            return GetMtlFileTextureByIndex(meshTextureName, GetMtlFileMaterialIndex(meshTextureName));
        }

        public int GetMtlFileMaterialIndex(string meshTextureName) {
            if (_currentMtlFilesTexturesIndex.TryGetValue(meshTextureName, out var index)) {
                return index;
            } else {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }
        }

        public IEnumerable<PlyModel3D> GetMtlFilePlyModels(string mtlFileName) {
            return _plyModels.Where(p => p.MeshesTextureNames.Contains(mtlFileName));
        }

        public void ClearPlyModelFocus() {
            FocusablePlyModel = null;
        }

        public void UpdateTextures() {
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

        private void UpdateLodLevels() {
            var maxLodLevel = _plyModels.Max(p => p.LodPlyFiles.Count);

            if (maxLodLevel + 1 == HumanskinLodLevels?.Count()) { return; }

            var array = new int[maxLodLevel + 1];

            for (int i = 0; i <= maxLodLevel; i++) {
                array[i] = i;
            }

            HumanskinLodLevel = 0;
            HumanskinLodLevels = array;
        }


        private void PlyModelChanged(object? s, EventArgs e) {
            var plyModel = (s as PlyModel3D)!;

            int index = _plyModels.IndexOf(plyModel);

            if (index >= 0 && index < _models.Count) {
                _models[_plyModels.IndexOf(plyModel)] = plyModel.Model;
            }

            UpdateLodLevels();
        }

        private void PlyModelRemoveHandler(object? sender, NotifyCollectionChangedEventArgs e) {
            if (_plyModels.Count == 0) {
                ClearAggregateFiles();
            } else {
                foreach (var aggregateMtlFile in _aggregateMtlFiles.Values) {
                    if (!GetMtlFilePlyModels(aggregateMtlFile.Name).Any()) {
                        RemoveAggregateMtlFile(aggregateMtlFile.Name);
                    }
                }
            }

            e.GetItem<PlyModel3D>()?.UnloadResource();
        }
    }
}
