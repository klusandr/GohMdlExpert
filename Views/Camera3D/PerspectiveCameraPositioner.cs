using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Views.Camera3D {
    public class PerspectiveCameraPositioner : INotifyPropertyChanged {
        private readonly PerspectiveCamera _camera;
        private readonly AxisAngleRotation3D _cameraYRotation;
        private readonly AxisAngleRotation3D _cameraXRotation;
        private readonly RotateTransform3D _cameraRotationYTransform;
        private readonly RotateTransform3D _cameraRotationXTransform;
        private Point3D _focus;

        public Point3D Focus {
            get => _focus; set {
                _focus = value;
                UpdateCameraLookDirection();
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

        public void RotationYCameraAroundFocus(double angle) {
            _cameraYRotation.Angle += angle * -1;
        }

        public void RotationXCameraAroundFocus(double angle) {
            _cameraXRotation.Angle += angle;
        }

        public void ZoomCamera(int value) {
            var moveVector = _camera.Position - Focus;
            var newPosition = _camera.Position;

            value *= -1;

            newPosition.X += moveVector.X * value / 10;
            newPosition.Y += moveVector.Y * value / 10;
            newPosition.Z += moveVector.Z * value / 10;
            _camera.Position = newPosition;
        }

        public void SetCameraFocus(Point3D point) {
            var d = _camera.Position;
            Focus = point;
            _camera.Position = new Point3D(point.X, point.Y, d.Z);
            //RotationXCameraAroundFocus(10);
            UpdateCameraLookDirection();

        }

        public void MoveCamera(Vector3D vector) {
            var newCameraPosition = _camera.Position;

            Vector3D focusVector = Focus - _camera.Position;

            newCameraPosition += vector;
            //Focus += vector;

            _camera.Position = newCameraPosition;
        }

        private void UpdateCameraLookDirection() {
            _camera.LookDirection = Focus - _camera.Position;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
