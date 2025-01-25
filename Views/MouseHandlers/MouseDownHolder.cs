using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace GohMdlExpert.Views.MouseHandlers {
    public class MouseDownHolder {
        public class MouseMoveArgs(Vector vector) : EventArgs {
            public Vector Vector { get; private set; } = vector;
        }

        private readonly FrameworkElement _frameworkElement;
        private Point _mousePosition;
        private Point _startMousePosition;
        private bool _mouseReturn;

        private Point FrameworkElementCenter => new(_frameworkElement.ActualWidth / 2, _frameworkElement.ActualHeight / 2);
        private Point FrameworkElementPosition => _frameworkElement.PointToScreen(new Point());

        public MouseButton NeedMouseButton { get; set; }

        public event Action<object, MouseMoveArgs>? MouseDownMove;

        public MouseDownHolder(FrameworkElement frameworkElement, MouseButton mouseButton = MouseButton.Left) {
            _frameworkElement = frameworkElement;
            NeedMouseButton = mouseButton;
            _frameworkElement.MouseMove += OnMouseMove;
            _frameworkElement.MouseDown += OnMouseButtonDown;
            _frameworkElement.MouseUp += OnMouseButtonUp;
        }

        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == NeedMouseButton) {
                _mousePosition = _startMousePosition = e.GetPosition(Application.Current.MainWindow);
                Mouse.OverrideCursor = Cursors.None;
                Mouse.Capture(_frameworkElement);
            }
        }

        private void OnMouseButtonUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == NeedMouseButton) {
                Mouse.OverrideCursor = Cursors.Arrow;
                SetCursorPos((int)(FrameworkElementPosition.X + _startMousePosition.X), (int)(FrameworkElementPosition.Y + _startMousePosition.Y));
                Mouse.Capture(null);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            if (GetMouseButtonState(e, NeedMouseButton) == MouseButtonState.Pressed) {
                var newMousePosition = e.GetPosition(_frameworkElement);

                if (_mouseReturn) {
                    _mouseReturn = false;
                    _mousePosition = newMousePosition;
                    return;
                }

                MouseDownMove?.Invoke(sender, new MouseMoveArgs(newMousePosition - _mousePosition));
                _mousePosition = newMousePosition;

                PreventMouseLeave(e.GetPosition(_frameworkElement));
            }
        }

        private MouseButtonState GetMouseButtonState(MouseEventArgs e, MouseButton mouseButton) {
            return mouseButton switch {
                MouseButton.Left => e.LeftButton,
                MouseButton.Right => e.RightButton,
                MouseButton.Middle => e.MiddleButton,
                MouseButton.XButton1 => e.XButton1,
                MouseButton.XButton2 => e.XButton2,
                _ => MouseButtonState.Released,
            };
        }

        private void PreventMouseLeave(Point mousePosition) {
            bool RightLeave = mousePosition.X > _frameworkElement.ActualWidth * 0.9;
            bool LeftLeave = mousePosition.X < _frameworkElement.ActualWidth * 0.1;
            bool bottomLeave = mousePosition.Y > _frameworkElement.ActualHeight * 0.9;
            bool topLeave = mousePosition.Y < _frameworkElement.ActualHeight * 0.1;

            if (RightLeave || LeftLeave || bottomLeave || topLeave) {
                var newMousePosition = mousePosition;

                if (RightLeave) {
                    newMousePosition.X = _frameworkElement.ActualWidth * 0.15;
                } else if (LeftLeave) {
                    newMousePosition.X = _frameworkElement.ActualWidth * 0.85;
                } else if (bottomLeave) {
                    newMousePosition.Y = _frameworkElement.ActualHeight * 0.15;
                } else if (topLeave) {
                    newMousePosition.Y = _frameworkElement.ActualHeight * 0.85;
                }

                SetCursorPos((int)(newMousePosition.X + FrameworkElementPosition.X), (int)(newMousePosition.Y + FrameworkElementPosition.Y));
                _mouseReturn = true;
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
    }
}
