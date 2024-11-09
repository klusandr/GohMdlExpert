using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        

        public MainWindow() {
            InitializeComponent();
            
            _mouseCameraRotatedMover = new(Scene, MouseButton.Left);
            _mouseCameraPositionMover = new(Scene, MouseButton.Middle);
            _cameraPositioner = new(camera);

            _mouseCameraRotatedMover.MouseDownMove += OnMouseCameraRotationMove;
            _mouseCameraPositionMover.MouseDownMove += OnMouseCameraPositionMover;

            var object3d = new ModelVisual3D() { Content = OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_03_unteroffizier.ply") };
            var object3d2 = new ModelVisual3D() { Content = OpenPlyFile("F:\\SDK\\Content\\goh\\humanskin\\[germans]\\[ger_source]\\ger_tunic_pzwrap\\ger_ranks\\heer_ranks\\ger_rank_heer_pzwrap_em_00a_oberschutze.ply") };

            var center = GetModelCenterPoint((GeometryModel3D)object3d.Content);


            Scene.Children.Add(object3d);
            Scene.Children.Add(object3d2);

            _cameraPositioner.SetCameraFocus(center);


            //camera.Position = center - new Vector3D(0, 0, 20);
            //camera.LookDirection = new Vector3D(0, 0, 1);

            //camera.Transform = new Transform3DGroup() {
            //    Children = {
            //        new RotateTransform3D(cameraRotation, center),
            //        //new RotateTransform3D(cameraRotation, camera.Position),
            //    }
            //};

            ScrolX.Value = camera.LookDirection.X;
            ScrolY.Value = camera.LookDirection.Y;
            ScrolZ.Value = camera.LookDirection.Z;
        }

        

        private GeometryModel3D OpenPlyFile(string fileName) {
            PlyReader.PlyRead(fileName);

            var mesh = new MeshGeometry3D();

            foreach (var point in PlyReader.Points) {
                mesh.Positions.Add(point);
            }

            foreach (var normal in PlyReader.Normalizes) {
                mesh.Normals.Add(normal);
            }

            mesh.TriangleIndices = new Int32Collection(PlyReader.IndicesList);

            GeometryModel3D model3D = new GeometryModel3D() {
                Geometry = mesh,
                Material = new DiffuseMaterial(new SolidColorBrush(new System.Windows.Media.Color() { A = 255, R = 255, G = 0, B = 0 }))
            };

            return model3D;
        }

       
        private void Scrol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            //camera.LookDirection = new Vector3D(ScrolX.Value, ScrolY.Value, ScrolZ.Value);
        }
    }
}
