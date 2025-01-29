using System.Windows;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Properties;
using GohMdlExpert.Services;
using GohMdlExpert.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm;
using WpfMvvm.DependencyInjection;
using WpfMvvm.ViewModels.Commands;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication {
        public App() { }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Settings.Default.Save();
        }

        protected override void OnServicesStartup(object? sender, ServicesStartupArgs e) {
            base.OnServicesStartup(sender, e);

            e.Services
                .AddSingleton<GohResourceProvider>()
                .AddSingleton<GohHumanskinResourceProvider>()
                .AddSingleton<GohTextureProvider>()
                .AddSingleton<MaterialSelector>()
                .AddSingleton((sp) => new CommandFactory(
                    exceptionHandler:
                    (e) => {
                        sp.GetRequiredService<IUserDialogProvider>().ShowError("", exception: e);
                    }
                ));

            ViewModelsStartup.Startup(sender, e);
        }

    }
}
