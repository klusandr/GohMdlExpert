using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Views.Camera3D {
    public class PerspectiveCameraPositioner : INotifyPropertyChanged {
        private PerspectiveCamera _camera;
        private Point3D _focus;
        readonly AxisAngleRotation3D _cameraYRotation;
        readonly AxisAngleRotation3D _cameraXRotation;
        readonly RotateTransform3D _cameraRotationYTransform;
        readonly RotateTransform3D _cameraRotationXTransform;

        public Point3D Focus {
            get => _focus; set {
                _focus = value;
                OnPropertyChanged(nameof(Focus));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PerspectiveCameraPositioner(PerspectiveCamera camera) {
            _camera = camera;
            Focus = new Point3D();
            _cameraYRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            _cameraXRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);

            _cameraRotationYTransform = new RotateTransform3D(_cameraYRotation, Focus);
            _cameraRotationXTransform = new RotateTransform3D(_cameraXRotation, Focus);

            _camera.Transform = new Transform3DGroup() {
                Children = {
                    _cameraRotationXTransform,              
                    _cameraRotationYTransform,
                }
            };

            PropertyChanged += (_, e) => {
                if (e.PropertyName == nameof(Focus)) {
                    _cameraRotationXTransform.CenterX = Focus.X;
                    _cameraRotationXTransform.CenterY = Focus.Y;
                    _cameraRotationXTransform.CenterZ = Focus.Z;

                    _cameraRotationYTransform.CenterX = Focus.X;
                    _cameraRotationYTransform.CenterY = Focus.Y;
                    _cameraRotationYTransform.CenterZ = Focus.Z;
                }
            };
        }

        public void RotationCameraAroundFocus(double Angle) {
            _cameraYRotation.Angle += Angle;
        }

        public void RotationXCameraAroundFocus(double Angle) {
            _cameraXRotation.Angle += Angle;
        }

        public void ZoomCamera(int value) {
            var moveVector = _camera.Position - Focus;
            var newPosition = _camera.Position;

            newPosition.X += moveVector.X / 10;
            newPosition.Y += moveVector.Y / 10;
            newPosition.Z += moveVector.Z / 10;
            _camera.Position = newPosition;
        }

        public void SetCameraFocus(Point3D point) {
            Focus = _camera.Position = point;
            _camera.Position += new Vector3D(0, 5, -20);
            _camera.LookDirection = Focus - _camera.Position;

        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
