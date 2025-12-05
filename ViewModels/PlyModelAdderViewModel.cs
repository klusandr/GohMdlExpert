using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels
{
    public class PlyModelAdderViewModel : BaseViewModel {
        private readonly HumanskinMdlOverviewViewModel _models3DView;
        private readonly DefaultTextureViewModel _defaultMaterialViewModel;
        private AggregateMtlFiles? _aggregateMtlFiles;
        private PlyModel3D? _addedModel;

        public PlyModel3D? AddedModel {
            get { return _addedModel; }
            private set {
                _addedModel = value;
                OnPropertyChanged();
            }
        }

        public AggregateMtlFiles? AggregateMtlFiles { get => _aggregateMtlFiles; set => _aggregateMtlFiles = value; }

        public bool IsAddedInProgress => AddedModel != null;

        public ICommand AddModelCommand => CommandManager.GetCommand(AddModel, canExecute: (_) => IsAddedInProgress);
        public ICommand ClearModelCommand => CommandManager.GetCommand(ClearModel, canExecute: (_) => IsAddedInProgress);

        public event EventHandler? ModelAdded;
        public event EventHandler? CancelModelAdded;

        public PlyModelAdderViewModel(HumanskinMdlOverviewViewModel models3DView, DefaultTextureViewModel defaultMaterialViewModel) {
            _models3DView = models3DView;
            _defaultMaterialViewModel = defaultMaterialViewModel;

            PropertyChangeHandler.AddHandler(nameof(AddedModel), (_, _) => {
                ((Command)AddModelCommand).OnCanExecuteChanged();
                ((Command)ClearModelCommand).OnCanExecuteChanged();
            });

            _defaultMaterialViewModel.PropertyChangeHandler.AddHandlerBuilder(nameof(DefaultTextureViewModel.IsUse), (_, _) => DefaultTextureUpdate());
            _defaultMaterialViewModel.PropertyChangeHandler.AddHandlerBuilder(nameof(DefaultTextureViewModel.IsUseAlways), (_, _) => DefaultTextureUpdate());
            //_defaultMaterialViewModel.PropertyChangeHandler.AddHandlerBuilder(nameof(DefaultMaterialViewModel.MaterialFile), (_, _) => DefaultTextureUpdate());
            _defaultMaterialViewModel.TexturesUpdate += (_, _) => DefaultTextureUpdate();
        }

        public void SetModel(PlyFile plyFile, AggregateMtlFiles? aggregateMtlFiles, IEnumerable<PlyFile>? lodPlyFiles) {
            ClearModel();

            if (aggregateMtlFiles != null && aggregateMtlFiles.PlyFile != plyFile) {
                throw TextureException.NotBelongPlyModel(aggregateMtlFiles);
            }

            AggregateMtlFiles = aggregateMtlFiles;
            AddedModel = new PlyModel3D(plyFile, aggregateMtlFiles, lodPlyFiles);

            AddedModel.ModelChanged += AddedModelModelChangedHandler;

            DefaultTextureUpdate();

            if (_models3DView.Autofocus) {
                _models3DView.FocusablePlyModel = _addedModel;
            }
        }

        private void AddedModelModelChangedHandler(object? sender, EventArgs e) {
            OnPropertyChanged(nameof(AddedModel));
        }

        public void SetMtlFiles(AggregateMtlFiles aggregateMtlFiles) {
            if (!IsAddedInProgress) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            if (aggregateMtlFiles.PlyFile != AddedModel!.PlyFile) {
                throw TextureException.NotBelongPlyModel(aggregateMtlFiles);
            }

            foreach (var aggregateMtlFile in aggregateMtlFiles) {
                AddedModel.SetMeshTexture(aggregateMtlFile.Name, aggregateMtlFile.Data.FirstOrDefault());
            }

            AggregateMtlFiles = aggregateMtlFiles;
        }

        public void ClearModel() {
            if (IsAddedInProgress) {
                AggregateMtlFiles = null;
                AddedModel = null;
                CancelModelAdded?.Invoke(this, EventArgs.Empty);

                if (_models3DView.Autofocus) {
                    _models3DView.ClearPlyModelFocus();
                }
            }
        }

        public void AddModel() {
            if (AddedModel != null) {
                try {
                    Dictionary<string, MtlTexture>? textures = null;

                    if (AggregateMtlFiles != null) {
                        textures = [];
                        foreach (var aggregateMtlFile in AggregateMtlFiles) {
                            var texture = AddedModel.GetMeshTexture(aggregateMtlFile.Name);

                            if (texture != null) {
                                if (aggregateMtlFile.Data.Count == 0 || _defaultMaterialViewModel.IsUseAlways) {
                                    aggregateMtlFile.Data.Add(texture);
                                }

                                textures.Add(aggregateMtlFile.Name, texture);
                            }
                        }
                    }

                    AddedModel.SetLodIndex(0);

                    _models3DView.AddModel(AddedModel, AggregateMtlFiles);

                    if (textures != null) {
                        foreach (var texture in textures) {
                            int index = _models3DView.AggregateMtlFiles[texture.Key].Data.IndexOf(texture.Value);
                            _models3DView.SetMtlFileTextureByIndex(texture.Key, index);
                        }
                    }

                    AddedModel = null;
                    ModelAdded?.Invoke(this, EventArgs.Empty);
                } catch (OperationCanceledException) { }

                if (_models3DView.Autofocus) {
                    _models3DView.ClearPlyModelFocus();
                }
            }
        }

        public void AddModel(PlyFile plyFile, AggregateMtlFiles? aggregateMtlFiles) {
            _models3DView.AddModel(new PlyModel3D(plyFile, aggregateMtlFiles), aggregateMtlFiles);
        }

        public void SelectModelMeshTexture(string mashTextureName, MtlTexture mtlTexture) {
            if (_defaultMaterialViewModel.IsUse && _defaultMaterialViewModel.IsUseAlways) {
                return;
            }

            if (!IsAddedInProgress) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            ResourceChecking.ThrowCheckPlyFileMeshTextureName(AddedModel!.PlyFile, mashTextureName);

            if (AggregateMtlFiles != null) {
                if (!AggregateMtlFiles[mashTextureName].Data.Contains(mtlTexture)) {
                    throw PlyModelException.AttemptInstallInvalidMtlTexture(AddedModel.PlyFile, mtlTexture);
                }
            }

            AddedModel!.SetMeshTexture(mashTextureName, mtlTexture);
        }

        private void SetTextureOnAggregateMtlFile() {
            if (AddedModel != null) {
                foreach (var textureName in AddedModel.MeshesTextureNames) {
                    MtlTexture? texture = null;

                    if (AggregateMtlFiles != null && AggregateMtlFiles.FilesNames.Contains(textureName)) {
                        texture = AggregateMtlFiles[textureName].Data.FirstOrDefault();
                    }

                    AddedModel.SetMeshTexture(textureName, texture);
                }
            }
        }

        private void ReplaceNullTextureOnDefault() {
            if (AddedModel != null) {
                foreach (var textureName in AddedModel.MeshesTextureNames) {
                    if (AddedModel.GetMeshTexture(textureName) == null) {
                        var texture = _defaultMaterialViewModel.GetTextureFile(textureName);

                        if (texture != null) {
                            AddedModel.SetMeshTexture(textureName, texture);
                        }
                    }
                }
            }
        }

        private void SetDefaultTexture() {
            if (AddedModel != null) {
                foreach (var textureName in AddedModel.MeshesTextureNames) {
                    var texture = _defaultMaterialViewModel.GetTextureFile(textureName);

                    if (texture != null) {
                        AddedModel.SetMeshTexture(textureName, texture);
                    }
                }
            }
        }

        private void DefaultTextureUpdate() {
            SetTextureOnAggregateMtlFile();

            if (_defaultMaterialViewModel.IsUse) {
                if (!_defaultMaterialViewModel.IsUseAlways) {
                    ReplaceNullTextureOnDefault();
                } else {
                    SetDefaultTexture();
                }
            }
        }
    }
}
