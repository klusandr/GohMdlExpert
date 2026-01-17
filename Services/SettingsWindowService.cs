using GohMdlExpert.Views.SettingsPages;

namespace GohMdlExpert.Services {
    public class SettingsWindowService {
        private SettingsView? _settingsView;
        private ChildWindow? _childWindow;

        public SettingsWindowService() { }

        private void SettingsApprovedHandler(object? sender, EventArgs e) {
            _childWindow?.Close();
        }

        public void OpenSettings(string? pageName = null) {
            if (_settingsView == null) {
                _settingsView = new SettingsView();
                _settingsView.ViewModel.SettingsApproved += SettingsApprovedHandler;
            }

            if (pageName != null) {
                _settingsView.OpenPage(pageName);
            }

            _childWindow = new ChildWindow() {
                Owner = App.Current.MainWindow,
                Title = "Settings",
                Content = _settingsView,
                MinWidth = _settingsView.MinWidth,
                MinHeight = _settingsView.MinHeight,
            };

            _childWindow.ShowDialog();
        }
    }
}
