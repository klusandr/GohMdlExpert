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
        private PlyAggregateMtlFiles? _aggregateMtlFiles;

        public Models3DViewModel Models3DView { get; }

        public PlyModel3D? AddedModel {
            get { return Models3DView.AddedModel; }
            private set {
                Models3DView.AddedModel = value;
                OnPropertyChanged();
            }
        }

        public PlyAggregateMtlFiles? AggregateMtlFiles { get => _aggregateMtlFiles; set => _aggregateMtlFiles = value; }

        public bool IsAddedInProgress => AddedModel != null;

        public ICommand AddModelCommand => CommandManager.GetCommand(AddModel);

        public event EventHandler? ModelAdded;

        public ModelAdderViewModel(Models3DViewModel models3DView) {
            Models3DView = models3DView;
        }

        public void SetModel(PlyFile plyFile, PlyAggregateMtlFiles? aggragateMtlFiles) {
            if (aggragateMtlFiles != null && plyFile != aggragateMtlFiles.PlyFile) {
                throw TextureException.NotBelongPlyModel(aggragateMtlFiles);
            }
    
            ClearModel();

            AddedModel = new PlyModel3D(plyFile, aggragateMtlFiles);
            AggregateMtlFiles = aggragateMtlFiles;
        }

        public void ClearModel() {
            if (IsAddedInProgress) {
                AggregateMtlFiles = null;
                AddedModel = null;
            }
        }

        public void AddModel() {
            if (AddedModel != null) {
                Models3DView.AddModel(AddedModel, AggregateMtlFiles);
                AddedModel = null;
                ModelAdded?.Invoke(this, EventArgs.Empty);
            }
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
    }
}
