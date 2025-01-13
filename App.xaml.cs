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

namespace GohMdlExpert {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication {
        public App() {
            AppStartup.Startup(this, EventArgs.Empty);
        }
#warning переработать startup приложения.
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            Settings.Default.Save();
        }
    }
}
