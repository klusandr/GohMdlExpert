using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using GohMdlExpert.ViewModels;
using GohMdlExpert.ViewModels.SettingsPages;
using GohMdlExpert.Views;
using GohMdlExpert.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.Services {
    public class HumanskinSaveService {
        private readonly IUserDialogProvider _userDialog;
        private readonly SettingsWindowService _settingsWindow;
        private HumanskinSaveView _view;
        private HumanskinSaveViewModel _viewModel;
        private ChildWindow? _window;

        public HumanskinSaveService(IUserDialogProvider userDialog, SettingsWindowService settingsWindow) {
            _view = new HumanskinSaveView();
            _viewModel = _view.ViewModel;

            _viewModel.Saved += HumanskinSavedHandler;
            _viewModel.Canceled += HumanskinSaveCanceledHandler;
            _userDialog = userDialog;
            _settingsWindow = settingsWindow;
        }

        public void Save(MdlFile mdlFile, Dictionary<string, MtlTexture> mtlTextures) {
            if (_window != null) {
                throw new InvalidOperationException("Humansin save dialog is open already.");
            }

            if (_viewModel.ModIsLoaded!) {
                if (_userDialog.Ask(
                        "For to save humanskin need to specifed output mod, open the output mod settings now?\n",
                        "Output mod",
                        QuestionType.YesNo) == QuestionResult.Yes) {
                    _settingsWindow.OpenSettings(OutputModSettingsPageViewModel.PageName);
                } else {
                    return;
                }
            }

            try {
                _window = new ChildWindow() {
                    Title = "Humanskin saving",
                    Content = _view,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
                };

                _viewModel.SetHumanskin(mdlFile, mtlTextures);

                _window.ShowDialog();

            } finally {
                _window = null;
            }
        }

        private void HumanskinSaveCanceledHandler(object? sender, EventArgs e) {
            _window?.Close();
        }

        private void HumanskinSavedHandler(object? sender, EventArgs e) {
            _window?.Close();
        }
    }
}
