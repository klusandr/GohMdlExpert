using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.ViewModels.ModelsTree;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels
{
    public class ModelAdderViewModel : BaseViewModel
    {
        private Model3DPly? _modelPly;
        private Model3D? _addedModel;

        public Models3DViewModel Models3DView { get; }
        public GohHumanskinResourceProvider SkinResourceProvider { get; }

        public event EventHandler? ModelAdded;

        public ICommand AddModelCommand => CommandManager.GetCommand(AddModel);

        public Model3DPly? AddedModel
        {
            get { return _modelPly; }
            private set
            {
                _modelPly = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddedInProgress => AddedModel != null;

        public ModelAdderViewModel(Models3DViewModel models3DView, GohHumanskinResourceProvider skinResourceProvider)
        {
            _addedModel = null;
            Models3DView = models3DView;
            SkinResourceProvider = skinResourceProvider;
        }

        public void SetModel(PlyFile plyFile)
        {
            if (AddedModel != null)
            {
                Models3DView.RemoveModel(AddedModel);
            }

            AddedModel = new Model3DPly(plyFile, SkinResourceProvider.Current?.GetPlyAggregateMtlFiles(plyFile));
            Models3DView.AddModel(AddedModel);
        }

        public void SelectModelMeshTextureByIndex(PlyModel.Mesh mesh, int index)
        {
            if (AddedModel == null)
            {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            AddedModel!.SelectMeshTexture(mesh.TextureName, index);
        }

        public void AddModel()
        {
            if (AddedModel != null)
            {
                Models3DView.AddModel(AddedModel);
            }

            AddedModel = null;
        }
    }
}
