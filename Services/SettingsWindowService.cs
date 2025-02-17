using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.ViewModels.SettingsPages;
using GohMdlExpert.Views.SettingsPages;

namespace GohMdlExpert.Services {
    public class SettingsWindowService { 
        private ChildWindow? _childWindow;
        private SettingsView? _settingsView;

        public SettingsWindowService() {
            
        }

        public void OpenSettings(string? pageName = null) {
            _settingsView ??= new SettingsView();

            _childWindow = new ChildWindow() {
                Title = "Settings",
                Content = _settingsView,
                MinWidth = _settingsView.MinWidth,
                MinHeight = _settingsView.MinHeight,
            };

            _childWindow.ShowDialog();
        }


    }
}
