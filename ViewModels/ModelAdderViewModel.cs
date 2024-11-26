using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.ViewModels.ModelsTree;
using MvvmWpf.ViewModels;
using MvvmWpf.ViewModels.Commands;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.ViewModels {
    public class ModelAdderViewModel : ViewModelBase {
        private Model3DPly? _modelPly;
        private Model3D? _addedModel;

        public Models3DViewModel Models3DView { get; }

        public ICommand EndAddCommand => CommandManager.GetCommand(AddModel);

        public Model3D? AddedModel {
            get => _addedModel;
            set {
                _addedModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddedInProgress => AddedModel != null;

        public ModelAdderViewModel(Models3DViewModel models3DView) {
            Models3DView = models3DView;
            _addedModel = null;
            CommandManager.CommandFactory = new OperationCommandFactory(exceptionHandler: (e) => MessageBox.Show(e!.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        public void SetModel(PlyFile plyFile) {
            _modelPly = new Model3DPly(plyFile);

            AddedModel = _modelPly;
        }

        public void SelectModelMeshTextureByIndex(PlyModel.Mesh mesh, int index) {
            if (AddedModel == null) {
                throw new InvalidOperationException("Error setting texture model. Model not added yet.");
            }

            _modelPly!.SelectMeshTexture(mesh.TextureName, index);
        }

        public void AddModel() {
            if (_modelPly != null) {
                Models3DView.AddModel(_modelPly);
            }

            AddedModel = null;
            ViewModelManager.GetViewModel<ModelsTreeViewModel>()!.CancelApproveItems();
        }
    }
}
