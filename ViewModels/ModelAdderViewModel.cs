using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels {
    public class ModelAdderViewModel : BaseViewModel {
        private Model3DPly? _addedPly;

        public Models3DViewModel Models3DView { get; }
        public GohHumanskinResourceProvider SkinResourceProvider { get; }

        public event EventHandler? ModelAdded;

        public ICommand AddModelCommand => CommandManager.GetCommand(AddModel);

        public Model3DPly? AddedModel {
            get { return _addedPly; }
            private set {
                _addedPly = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddedInProgress => AddedModel != null;

        public ModelAdderViewModel(Models3DViewModel models3DView, GohHumanskinResourceProvider skinResourceProvider) {
            _addedPly = null;
            Models3DView = models3DView;
            SkinResourceProvider = skinResourceProvider;
        }

        public void SetModel(PlyFile plyFile) {
            if (AddedModel != null) {
                Models3DView.RemoveModel(AddedModel);
            }

            AddedModel = new Model3DPly(plyFile, SkinResourceProvider.Current?.GetPlyAggregateMtlFiles(plyFile));
            Models3DView.AddModel(AddedModel);
        }

        public void SelectModelMeshTextureByIndex(string mashTextureName, int index) {
            if (AddedModel == null) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            AddedModel!.SelectMeshTexture(mashTextureName, index);
        }

        public void AddModel() {
            AddedModel = null;
        }
    }
}
