using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Serialization;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels
{
    public class PlyModelAdderViewModel : BaseViewModel {
        private readonly HumanskinMdlOverviewViewModel _models3DView;
        private readonly DefaultMaterialViewModel _defaultMaterialViewModel;
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

        public PlyModelAdderViewModel(HumanskinMdlOverviewViewModel models3DView, DefaultMaterialViewModel defaultMaterialViewModel) {
            _models3DView = models3DView;
            _defaultMaterialViewModel = defaultMaterialViewModel;

            PropertyChangeHandler.AddHandler(nameof(AddedModel), (_, _) => {
                ((Command)AddModelCommand).OnCanExecuteChanged();
                ((Command)ClearModelCommand).OnCanExecuteChanged();
            });

            _defaultMaterialViewModel.PropertyChangeHandler.AddHandlerBuilder(nameof(DefaultMaterialViewModel.IsUse), (_, _) => DefaultTextureUpdate());
            _defaultMaterialViewModel.PropertyChangeHandler.AddHandlerBuilder(nameof(DefaultMaterialViewModel.IsUseAlways), (_, _) => DefaultTextureUpdate());
            _defaultMaterialViewModel.PropertyChangeHandler.AddHandlerBuilder(nameof(DefaultMaterialViewModel.MaterialFile), (_, _) => DefaultTextureUpdate());
        }

        public void SetModel(PlyFile plyFile, AggregateMtlFiles? aggregateMtlFiles) {
            ClearModel();

            if (aggregateMtlFiles != null && aggregateMtlFiles.PlyFile != plyFile) {
                throw TextureException.NotBelongPlyModel(aggregateMtlFiles);
            }

            AggregateMtlFiles = aggregateMtlFiles;
            AddedModel = new PlyModel3D(plyFile, aggregateMtlFiles);

            DefaultTextureUpdate();
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
            }
        }

        public void AddModel() {
            if (AddedModel != null) {
                try {
                    _models3DView.AddModel(AddedModel, AggregateMtlFiles);
                    AddedModel = null;
                    ModelAdded?.Invoke(this, EventArgs.Empty);
                } catch (OperationCanceledException) { }
            }
        }

        public void AddModel(PlyFile plyFile, AggregateMtlFiles? aggregateMtlFiles) {
            _models3DView.AddModel(new PlyModel3D(plyFile, aggregateMtlFiles), aggregateMtlFiles);
        }

        public void SelectModelMeshTexture(string mashTextureName, MtlTexture mtlTexture) {
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
                        if (_defaultMaterialViewModel.IsUse && _defaultMaterialViewModel.MaterialFile != null) {
                            AddedModel.SetMeshTexture(textureName, new MtlTexture(_defaultMaterialViewModel.MaterialFile));
                        }
                    }
                }
            }
        }

        private void SetDefaultTexture() {
            if (AddedModel != null) {
                foreach (var textureName in AddedModel.MeshesTextureNames) {
                    if (_defaultMaterialViewModel.IsUse && _defaultMaterialViewModel.MaterialFile != null) {
                        AddedModel.SetMeshTexture(textureName, new MtlTexture(_defaultMaterialViewModel.MaterialFile));
                    }
                }
            }
        }

        private void DefaultTextureUpdate() {
            if (_defaultMaterialViewModel.IsUse && _defaultMaterialViewModel.MaterialFile != null) {
                if (!_defaultMaterialViewModel.IsUseAlways) {
                    SetTextureOnAggregateMtlFile();
                    ReplaceNullTextureOnDefault();
                } else {
                    SetDefaultTexture();
                }
            } else {
                SetTextureOnAggregateMtlFile();
            }
        }
    }
}
