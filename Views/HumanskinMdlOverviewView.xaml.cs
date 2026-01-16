using System.Windows.Input;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Extensions;
using GohMdlExpert.ViewModels;
using GohMdlExpert.Views.Camera3D;
using GohMdlExpert.Views.MouseHandlers;
using WpfMvvm.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для Models3dView.xaml
    /// </summary>
    [BindingViewModel<HumanskinMdlOverviewViewModel>]
    [BindingViewModelViaDI]
    public partial class HumanskinMdlOverviewView : BaseView {
        private MouseDownHolder _mouseCameraRotatedMover;
        private MouseDownHolder _mouseCameraPositionMover;
        private PerspectiveCameraPositioner _cameraPositioner;

        private new HumanskinMdlOverviewViewModel ViewModel => (HumanskinMdlOverviewViewModel)base.ViewModel!;

        public HumanskinMdlOverviewView() {
            InitializeComponent();

            _mouseCameraRotatedMover = new(_sceneBackground, MouseButton.Left);
            _mouseCameraPositionMover = new(_sceneBackground, MouseButton.Middle);
            _cameraPositioner = new(_perspectivCamera);

            _mouseCameraRotatedMover.MouseDownMove += OnMouseCameraRotationMove;
            _mouseCameraPositionMover.MouseDownMove += OnMouseCameraPositionMover;

            _perspectivCamera.Position = new Point3D(0, 0, 40);
            _cameraPositioner.SetCameraFocus(new Point3D());

            var tr = new TranslateTransform3D(_cameraPositioner.Focus - new Point3D());

            _cameraPositioner.PropertyChanged += (_, e) => {
                if (e.PropertyName == "Focus") {
                    tr.OffsetX = _cameraPositioner.Focus.X;
                    tr.OffsetY = _cameraPositioner.Focus.Y;
                    tr.OffsetZ = _cameraPositioner.Focus.Z;
                }
            };

            //_scene.Children.Add(new ModelVisual3D() {
            //    Content = Geometry3DDrawinger.DrowCube(1),
            //    Transform = tr
            //});
        }

        public PerspectiveCamera Camera => _perspectivCamera;

        protected override void OnViewModelInitialized(BaseViewModel viewModel) {
            base.OnViewModelInitialized(viewModel);

            //ViewModel.Models.Changed += ((_, _) => _cameraPositioner.SetCameraFocus(ViewModel.Models.First(). ?? new Point3D()));

            ViewModel?.PropertyChangeHandler.AddHandler(nameof(HumanskinMdlOverviewViewModel.FocusablePlyModel), (_, _) => {
                if (ViewModel.FocusablePlyModel != null) {
                    var point = ViewModel.FocusablePlyModel.GetCenterPoint();
                    _cameraPositioner.SetCameraFocus(point);
                } else {
                    if (ViewModel.PlyModels.Any()) {
                        var point = ViewModel.PlyModels.Select(pm => pm.GetCenterPoint()).GetCenterPoint();
                        _cameraPositioner.SetCameraFocus(point);
                    }
                }
            });

        }

        private void OnMouseCameraPositionMover(object sender, MouseDownHolder.MouseMoveArgs e) {
            _cameraPositioner.MoveCamera(new Vector3D(-e.Vector.X, e.Vector.Y, 0) / 100);
        }

        private void OnMouseCameraRotationMove(object sender, MouseDownHolder.MouseMoveArgs e) {
            _cameraPositioner.RotationYCameraAroundFocus(e.Vector.X);
            _cameraPositioner.RotationXCameraAroundFocus(e.Vector.Y * -0.5);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
            _cameraPositioner.ZoomCamera(e.Delta / 120);
        }
    }
}
