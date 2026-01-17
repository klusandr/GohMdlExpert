using GohMdlExpert.Views.Dialogs;

namespace GohMdlExpert.Services {
    public class RequestTextService {
        private RequestTextDialogView? _view;
        private ChildWindow? _window;

        public string? Request(string? text = null, string? title = null, string? defaultValue = null) {
            if (_view != null || _window != null) {
                throw new InvalidOperationException("Request text dialog is open already.");
            }

            try {
                _view = new RequestTextDialogView();
                _view.DialogText.Text = text ?? "";
                _view.InputText.Text = defaultValue;

                _window = new ChildWindow() {
                    Title = title,
                    Content = _view,
                    SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                    ResizeMode = System.Windows.ResizeMode.NoResize,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
                };

                _view.OkButton.Click += ViewOkButtonClickHandler;
                _view.CancleButton.Click += ViewCancleButtonClickHandler;

                _window.ShowDialog();

                return _view.InputText.Text;
            } finally {
                _view = null;
                _window = null;
            }
        }

        private void ViewOkButtonClickHandler(object sender, System.Windows.RoutedEventArgs e) {
            _window?.Close();
        }

        private void ViewCancleButtonClickHandler(object sender, System.Windows.RoutedEventArgs e) {
            if (_view != null) {
                _view.InputText.Text = null;
            }

            _window?.Close();
        }
    }
}
