using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels {
    public class ModelAdderViewModel : BaseViewModel {
        private PlyModel3D? _addedModel;
        private PlyAggregateMtlFiles? _aggregateMtlFiles;

        public Models3DViewModel Models3DView { get; }

        public PlyModel3D? AddedModel {
            get { return _addedModel; }
            private set {
                _addedModel = value;
                OnPropertyChanged();
            }
        }

        public PlyAggregateMtlFiles? AggregateMtlFiles { get => _aggregateMtlFiles; set => _aggregateMtlFiles = value; }

        public bool IsAddedInProgress => AddedModel != null;

        public ICommand AddModelCommand => CommandManager.GetCommand(AddModel);


        public event EventHandler? ModelAdded;

        public ModelAdderViewModel(Models3DViewModel models3DView) {
            _addedModel = null;
            Models3DView = models3DView;
        }

        public void SetModel(PlyFile plyFile, PlyAggregateMtlFiles? mtlFiles) {
            if (mtlFiles != null && plyFile != mtlFiles.PlyFile) {
                throw new PlyModelException(plyFile, "Files .mtl don't belong to the ply model.");
            }
    
            ClearModel();

            AddedModel = new PlyModel3D(plyFile, mtlFiles);
            Models3DView.AddModel(AddedModel);
        }

        public void ClearModel() {
            if (IsAddedInProgress) {
                Models3DView.RemoveModel(AddedModel!);
                AggregateMtlFiles = null;
                AddedModel = null;
            }
        }

        public void AddModel() {
            AddedModel = null;
            ModelAdded?.Invoke(this, EventArgs.Empty);
        }

        public void SelectModelMeshTexture(string mashTextureName, MtlTexture mtlTexture) {
            if (!IsAddedInProgress) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            ResourceChecking.ThrowCheckPlyFileMeshTextureName(AddedModel!.PlyFile!, mashTextureName);

            if (AggregateMtlFiles != null) {
                if (!AggregateMtlFiles[mashTextureName].Data.Contains(mtlTexture)) {
                    throw PlyModelException.AttemptInstallInvalidMtlTexture(AddedModel.PlyFile, mtlTexture);
                }
            }

            AddedModel!.SetMeshTexture(mashTextureName, mtlTexture);
        }
    }
}
