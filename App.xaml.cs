using System.IO;
using System.Windows;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using GohMdlExpert.Properties;
using GohMdlExpert.Services;
using GohMdlExpert.ViewModels;
using GohMdlExpert.ViewModels.SettingsPages;
using GohMdlExpert.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm;
using WpfMvvm.DependencyInjection;
using WpfMvvm.Exceptions;
using WpfMvvm.ViewModels.Commands;
using WpfMvvm.Views;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication {
        private const string DUMP_FILE = "dump";

        public App() {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
        }
        public bool IsInitialized { get; private set; }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);

            var settings = Settings.Default;

            settings.Save();
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var settings = Settings.Default;
            settings.Load();
            AppThemesManager themesManager = ServiceProvider.GetRequiredService<AppThemesManager>();

            if (!string.IsNullOrEmpty(settings.ThemeName)) {
                themesManager.SetCurrentTheme(settings.ThemeName);
            }

            themesManager.ThemeChange += (_, _) => {
                settings.ThemeName = themesManager.CurrentThemeName ?? AppThemesManager.LightThemeName;
                settings.Save();
            };

            LoadSettings();
        }

        private void LogUnhandledException(Exception exceptionObject, string v) {
            string fileName = DUMP_FILE;

            int fileNumber = 1;
            while (File.Exists(fileName)) {
                fileName = DUMP_FILE + fileNumber++;
            }

            File.WriteAllText(fileName, ServiceProvider.GetRequiredService<ExceptionFormatter>().ExceptionToString(exceptionObject));
        }

        protected override void OnServicesStartup(object? sender, ServicesStartupArgs e) {
            base.OnServicesStartup(sender, e);

            e.Services
                .AddSingleton<IUserDialogProvider, UserDialogProviderGoh>()
                .AddSingleton<GohGameDirectory>()
                .AddSingleton<GohResourceProvider>()
                .AddSingleton<GohModsResourceProvider>()
                .AddSingleton<GohOutputModProvider>()
                .AddSingleton<GohHumanskinResourceProvider>()
                .AddSingleton<GohTextureProvider>()
                .AddSingleton<SettingsWindowService>()
                .AddSingleton<TextureLoadService>()
                .AddSingleton(s => new AppThemesManager()
                    .AddTheme(AppThemesManager.LightThemeName, AppThemesManager.LightThemePath)
                    .AddTheme(AppThemesManager.DarkThemeName, AppThemesManager.DarkThemePath)
                    .SetCurrentTheme(AppThemesManager.LightThemeName)
                )
                .AddSingleton((sp) => new CommandFactory(
                    exceptionHandler:
                    (e) => {
                        sp.GetRequiredService<IUserDialogProvider>().ShowError("", exception: e);
                    }
                ))
                .AddSingleton<SelectResourceFileService>()
                .AddSingleton<RequestTextService>()
                .AddSingleton<HumanskinSaveService>();
            ViewModelsStartup.Startup(sender, e);
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            Settings settings = Settings.Default;

            if (!IsInitialized && settings.LoadGameResourceOnStart) {
                var gameDirectory = ServiceProvider.GetRequiredService<GohGameDirectory>();

                gameDirectory.Updated += (_, _) => {
                    if (gameDirectory.ResourcePath != null) {
                        ServiceProvider.GetRequiredService<GohResourceProvider>().LoadGameResource(gameDirectory.ResourcePath);
                        ServiceProvider.GetRequiredService<ApplicationViewModel>().FullLoadResources();
                    }
                };

                if (settings.LoadGameResourceOnStart) {
                    if (!string.IsNullOrEmpty(settings.GameDirectoryPath)) {
                        try {
                            gameDirectory.Open(settings.GameDirectoryPath);
                        } catch (GohResourceLoadException ex) {
                            ServiceProvider.GetRequiredService<IUserDialogProvider>().ShowError(string.Empty, exception: ex);
                        }
                    } else {
                        if (ServiceProvider.GetRequiredService<IUserDialogProvider>().Ask(
                            "The path to the game directory is not specified, open the game directory settings now?\n" +
                            "You can disable automatic load of game resource in the settings.", "Game directory path",
                            QuestionType.YesNo) == QuestionResult.Yes) {
                            ServiceProvider.GetRequiredService<SettingsWindowService>().OpenSettings(GamePathSettingsPageViewModel.PageName);
                        }
                    }
                }

                IsInitialized = true;
            }
        }

        private void LoadSettings() {
            ServiceProvider.GetRequiredService<SettingsViewModel>().LoadSettings();
        }
    }
}
