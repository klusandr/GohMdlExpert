using System.Windows.Input;
using System.Windows.Media.Media3D;
using GohMdlExpert.ViewModels;
using GohMdlExpert.Views.Camera3D;
using GohMdlExpert.Views.Models3D;
using GohMdlExpert.Views.MouseHandlers;
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

            _cameraPositioner.SetCameraFocus(new Point3D());

            //Models3DViewModel.Models.Changed += ((_, _) => _cameraPositioner.SetCameraFocus(Models3DViewModel.ModelsCenter ?? new Point3D()));

            var tr = new TranslateTransform3D(_cameraPositioner.Focus - new Point3D());

            _cameraPositioner.PropertyChanged += (_, e) => {
                if (e.PropertyName == "Focus") {
                    tr.OffsetX = _cameraPositioner.Focus.X;
                    tr.OffsetY = _cameraPositioner.Focus.Y;
                    tr.OffsetZ = _cameraPositioner.Focus.Z;
                }
            };

            _scene.Children.Add(new ModelVisual3D() {
                Content = Geometry3DDrawinger.DrowCube(0.1),
                Transform = tr
            });
        }

        public PerspectiveCamera Camera => _perspectivCamera;

        private void OnMouseCameraPositionMover(object sender, MouseDownHolder.MouseMoveArgs e) {
            _cameraPositioner.MoveCamera(new Vector3D(e.Vector.X, e.Vector.Y, 0) / 100);
        }

        private void OnMouseCameraRotationMove(object sender, MouseDownHolder.MouseMoveArgs e) {
            _cameraPositioner.RotationCameraAroundFocus(e.Vector.X * 0.5);
            _cameraPositioner.RotationXCameraAroundFocus(e.Vector.Y * 0.5);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
            _cameraPositioner.ZoomCamera(e.Delta / 120);
        }
    }
}
