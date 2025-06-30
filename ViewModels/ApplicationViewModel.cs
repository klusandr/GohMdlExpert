using System.IO;
using System.Timers;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell;
using GohMdlExpert.Models.GatesOfHell.Caches;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using GohMdlExpert.Properties;
using GohMdlExpert.Services;
using GohMdlExpert.Views;
using Microsoft.Win32;
using WpfMvvm.Extensions;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;
using WpfMvvm.Views;

namespace GohMdlExpert.ViewModels {
    public class ApplicationViewModel : BaseViewModel {
        private readonly GohResourceProvider _gohResourceProvider;
        private readonly GohHumanskinResourceProvider _gohHumanskinResourceProvider;
        private readonly GohTextureProvider _gohTextureProvider;
        private readonly HumanskinMdlOverviewViewModel _models3DView;
        private readonly SettingsWindowService _settingsWindowService;
        private readonly AppThemesManager _appThemesManager;
        private readonly GohModResourceProvider _modResourceProvider;
        private bool _isWaitFillVisible;

        public float CompletionPercentage { get; set; }

        public bool IsWaitFillVisible {
            get => _isWaitFillVisible;
            private set {
                _isWaitFillVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenResourceCommand => CommandManager.GetCommand(OpenResourceDirectory);
        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenFile);
        public ICommand OpenSettingsCommand => CommandManager.GetCommand(OpenSettings);
        public ICommand LoadPlyTexturesCacheCommand => CommandManager.GetCommand(LoadPlyTexturesCache);
        public ICommand LoadTexturesCacheCommand => CommandManager.GetCommand(LoadTexturesCache);
        public ICommand SetLightThemeCommand => CommandManager.GetCommand(() => SetTheme(AppThemesManager.LightThemeName));
        public ICommand SetDarkThemeCommand => CommandManager.GetCommand(() => SetTheme(AppThemesManager.DarkThemeName));
        public ICommand TestCommand => CommandManager.GetCommand(Test);

        public ApplicationViewModel(GohResourceProvider gohResourceProvider, GohHumanskinResourceProvider gohHumanskinResourceProvider, GohTextureProvider gohTextureProvider,
            HumanskinMdlOverviewViewModel models3DView, SettingsWindowService settingsWindowService, AppThemesManager appThemesManager, GohModResourceProvider modResourceProvider) {
            _gohResourceProvider = gohResourceProvider;
            _models3DView = models3DView;
            _settingsWindowService = settingsWindowService;
            _appThemesManager = appThemesManager;
            _modResourceProvider = modResourceProvider;
            _gohHumanskinResourceProvider = gohHumanskinResourceProvider;
            _gohTextureProvider = gohTextureProvider;
        }

        public void OpenFile() {
            var fileDialog = new OpenFileDialog {
                Filter = GohResourceLoading.MdlFileOpenFilter,
                InitialDirectory = Path.GetDirectoryName(Settings.Default.LastOpenedFile)
            };

            if (fileDialog.ShowDialog() ?? false) {
                Settings.Default.LastOpenedFile = fileDialog.FileName;
                _models3DView.SetMtlFile(new MdlFile(fileDialog.FileName));
            }
        }

        public void OpenResourceDirectory() {
            var folderDialog = new OpenFolderDialog() {
                InitialDirectory = Settings.Default.LastOpenedResource
            };

            if (folderDialog.ShowDialog() ?? false) {
                Settings.Default.LastOpenedResource = folderDialog.FolderName;
                _gohResourceProvider.OpenResources(folderDialog.FolderName);
                FullLoadResources();
            }

            //if (_modResourceProvider.Mods.Any()) {
            //    _modResourceProvider.Mods.First().IsEnable = false;
            //    _gohResourceProvider.LoadModResources();
            //} else {
            //    if (folderDialog.ShowDialog() ?? false) {
            //        Settings.Default.LastOpenedResource = folderDialog.FolderName;
            //        _modResourceProvider.AddMod(new(folderDialog.FolderName));
            //        _gohResourceProvider.LoadModResources();
            //    }
            //}
        }

        public void LoadPlyTexturesCache() {
            float completionPercentage = 0;

            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (_, _) => {
                CompletionPercentage = completionPercentage;
                OnPropertyChanged(nameof(CompletionPercentage));
            };

            timer.AutoReset = true;
            timer.Enabled = true;

            Task.Factory.StartNew(() => {
                GohCachesFilling.FillPlyTexturesCache(_gohResourceProvider, _gohHumanskinResourceProvider.Resource, ref completionPercentage);
                completionPercentage = 0;
            }).ContinueWith((t) => timer.Dispose());
        }

        public void FullLoadResources(bool autoClose = true) {
            var viewModel = new ResourceLoadingProgressViewModel(_gohResourceProvider);
            var window = new ChildWindow() {
                Content = new ResourceLoadingProgressView() {
                    DataContext = viewModel
                },
                SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                Title = "Loading resources...",
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
            };

            Task.Factory.StartNew(() => {
                Thread.Sleep(200);
                _gohResourceProvider.FullLoad(viewModel.LoadElementHandler);
                viewModel.EndLoadingHandler();

                if (autoClose) {
                    Thread.Sleep(200);
                    App.Current.Synchronize(window.Close);
                }
            });

            try {
                IsWaitFillVisible = true;
                App.Current.Synchronize(window.ShowDialog);
            } finally {
                IsWaitFillVisible = false;
            }
        }

        private void OpenSettings() {
            _settingsWindowService.OpenSettings();
        }

        private void LoadTexturesCache(object? obj) {
            float completionPercentage = 0;

            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (_, _) => {
                CompletionPercentage = completionPercentage;
                OnPropertyChanged(nameof(CompletionPercentage));
            };

            timer.AutoReset = true;
            timer.Enabled = true;

            Task.Factory.StartNew(() => {
                GohCachesFilling.FillTexturesCache(_gohTextureProvider, _gohHumanskinResourceProvider.Resource, ref completionPercentage);
                completionPercentage = 0;
            }).ContinueWith((t) => timer.Dispose());
        }

        private void SetTheme(string themeName) {
            _appThemesManager.SetCurrentTheme(themeName);
        }

        private void Test() {
            FullLoadResources();
        }
    }
}
