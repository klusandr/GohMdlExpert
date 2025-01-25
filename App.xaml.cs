using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfMvvm.ViewModels;
using WpfMvvm.DependencyInjection;
using WpfMvvm;
using GohMdlExpert.ViewModels;
using GohMdlExpert.Models.GatesOfHell.Resources;
using WpfMvvm.Diagnostics;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Services;
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

        protected override void OnServicesStartup(object? sender, DependencyInjectionStatupArgs e) {
            base.OnServicesStartup(sender, e);

            e.Services.AddSingleton<GohResourceLocations>();
            e.Services.AddSingleton<GohResourceProvider>();
            e.Services.AddSingleton<GohHumanskinResourceProvider>();
            e.Services.AddSingleton<GohTextureProvider>();

            e.Services.AddSingleton<MaterialSelector>();

            e.Services.AddSingleton(new CommandFactory(
                exceptionHandler: (e) => {
                    ServiceProvider.GetRequiredService<IUserDialogProvider>().ShowError("", exception: e);
                }
            ));

            ViewModelsStartup.Startup(sender, e);
        }

    }
}
