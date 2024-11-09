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

namespace _3dViewWPF {
    /// <summary>
    /// Логика взаимодействия для Scene.xaml
    /// </summary>
    public partial class Scene : UserControl {
        private MouseDownHolder _mouseCameraRotatedMover;
        private MouseDownHolder _mouseCameraPositionMover;
        private PerspectiveCameraCameraPositioner _cameraPositioner;

        public PerspectiveCamera Camera => _perspectivCamera;

        public Scene() {
            InitializeComponent();

            _mouseCameraRotatedMover = new(Scene, MouseButton.Left);
            _mouseCameraPositionMover = new(Scene, MouseButton.Middle);
            _cameraPositioner = new(_perspectivCamera);

            _mouseCameraRotatedMover.MouseDownMove += OnMouseCameraRotationMove;
            _mouseCameraPositionMover.MouseDownMove += OnMouseCameraPositionMover;

            //var object3d = new ModelVisual3D() { Content = OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_03_unteroffizier.ply") };
            //var object3d2 = new ModelVisual3D() { Content = OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_00a_oberschutze.ply") };

            var center = GetModelCenterPoint((GeometryModel3D)object3d.Content);


            Scene.Children.Add(object3d);
            Scene.Children.Add(object3d2);

            _cameraPositioner.SetCameraFocus(center);
        }

        public void Add3dModel(Model3D model) {
            Scene.Children.Add(new ModelVisual3D() { Content = model };
        }

        private void OnMouseCameraPositionMover(object sender, MouseMoveArgs e) {
            var newCameraPosition = _perspectivCamera.Position;
            newCameraPosition.X += e.Vector.X / 1000;
            newCameraPosition.Y += e.Vector.Y / 1000;
            Debug.Print(e.Vector.ToString());
            _perspectivCamera.Position = newCameraPosition;
        }

        private void OnMouseCameraRotationMove(object sender, MouseMoveArgs e) {
            _cameraPositioner.RotationCameraAroundFocus((e.Vector.X < 0) ? 1 : -1);
            _cameraPositioner.RotationXCameraAroundFocus((e.Vector.Y > 0) ? 0.5 : -0.5);
        }

        private Point3D GetModelCenterPoint(GeometryModel3D model3d) {
            var mesh = (MeshGeometry3D)model3d.Geometry;

            return new Point3D() {
                X = mesh.Positions.Average(x => x.X),
                Y = mesh.Positions.Average(x => x.Y),
                Z = mesh.Positions.Average(x => x.Z),
            };
        }
    }
}
