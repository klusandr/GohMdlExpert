using System.Windows;
using System.Windows.Navigation;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using GohMdlExpert.Properties;
using GohMdlExpert.Services;
using GohMdlExpert.ViewModels;
using GohMdlExpert.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm;
using WpfMvvm.DependencyInjection;
using WpfMvvm.Extensions;
using WpfMvvm.ViewModels.Commands;
using WpfMvvm.Views;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication {
        public App() { }
        public bool IsInitialized { get; private set; }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);

            var settings = Settings.Default;

            settings.ThemeName = ServiceProvider.GetRequiredService<AppThemesManager>().CurrentThemeName ?? AppThemesManager.LightThemeName;

            Settings.Default.Save();
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var settings = Settings.Default;

            if (!string.IsNullOrEmpty(settings.ThemeName)) {
                ServiceProvider.GetRequiredService<AppThemesManager>().SetCurrentTheme(settings.ThemeName);
            }
        }

        protected override void OnServicesStartup(object? sender, ServicesStartupArgs e) {
            base.OnServicesStartup(sender, e);

            e.Services
                .AddSingleton<IUserDialogProvider, UserDialogProviderGoh>()
                .AddSingleton<GohGameDirectory>()
                .AddSingleton<GohResourceProvider>()
                .AddSingleton<GohModResourceProvider>()
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
                ));

            ViewModelsStartup.Startup(sender, e);
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);

            if (!IsInitialized && Settings.Default.LoadGameResourceOnStart) {
                var gameDirectory = ServiceProvider.GetRequiredService<GohGameDirectory>();

                gameDirectory.Updated += (_, _) => {
                    if (gameDirectory.ResourcePath != null) {
                        ServiceProvider.GetRequiredService<GohResourceProvider>().OpenResources(gameDirectory.ResourcePath);
                        ServiceProvider.GetRequiredService<ApplicationViewModel>().FullLoadResources();
                    }  
                };

                if (!string.IsNullOrEmpty(Settings.Default.GameDirectoryPath)) {
                    try {
                        gameDirectory.Open(Settings.Default.GameDirectoryPath);
                    } catch (GohResourceLoadException ex) {
                        ServiceProvider.GetRequiredService<IUserDialogProvider>().ShowError(string.Empty, exception: ex);
                    }
                } else {
                    if (ServiceProvider.GetRequiredService<IUserDialogProvider>().Ask("The path to the game directory is not specified, open the game directory settings now?", "Game directory path", QuestionType.YesNo) == QuestionResult.Yes) {
                        ServiceProvider.GetRequiredService<SettingsWindowService>().OpenSettings();
                    }
                }
                IsInitialized = true;
            }
        }
    }
}
