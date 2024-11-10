using GohMdlExpert.ViewModesl;
using GohMdlExpert.Views.Camera3D;
using GohMdlExpert.Views.MouseHandlers;
using MvvmWpf.ViewModels;
using MvvmWpf.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GohMdlExpert.Views
{
    /// <summary>
    /// Логика взаимодействия для Models3dView.xaml
    /// </summary>
    public partial class Models3DView : ViewMvvmBase {
        private MouseDownHolder _mouseCameraRotatedMover;
        private MouseDownHolder _mouseCameraPositionMover;
        private PerspectiveCameraPositioner _cameraPositioner;

        private Models3DViewModel Models3DViewModel => (Models3DViewModel)ViewModel!;

        public Models3DView() {
            InitializeComponent();

            ViewModel = new Models3DViewModel();

            _mouseCameraRotatedMover = new(_sceneBackground, MouseButton.Left);
            _mouseCameraPositionMover = new(_sceneBackground, MouseButton.Middle);
            _cameraPositioner = new(_perspectivCamera);

            _mouseCameraRotatedMover.MouseDownMove += OnMouseCameraRotationMove;
            _mouseCameraPositionMover.MouseDownMove += OnMouseCameraPositionMover;

            _cameraPositioner.SetCameraFocus(Models3DViewModel.ModelsCenter ?? new Point3D());
        }

        public PerspectiveCamera Camera => _perspectivCamera;

        private void OnMouseCameraPositionMover(object sender, MouseDownHolder.MouseMoveArgs e) {
            var newCameraPosition = _perspectivCamera.Position;
            newCameraPosition.X += e.Vector.X / 1000;
            newCameraPosition.Y += e.Vector.Y / 1000;
            Debug.Print(e.Vector.ToString());
            _perspectivCamera.Position = newCameraPosition;
        }

        private void OnMouseCameraRotationMove(object sender, MouseDownHolder.MouseMoveArgs e) {
            _cameraPositioner.RotationCameraAroundFocus((e.Vector.X < 0) ? 1 : -1);
            _cameraPositioner.RotationXCameraAroundFocus((e.Vector.Y > 0) ? 0.5 : -0.5);
        }
    }
}
