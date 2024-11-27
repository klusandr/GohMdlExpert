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

namespace GohMdlExpert {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : WpfApplication {

        public static new App Current => (App)WpfApplication.Current;

        public App() {
            AppDependencyInjection.ServicesStartup += GohMdlExpert.ServicesStartup.Startup;
            ViewModelProvider.ViewModelsProviderStartup += ViewModelsStartup.Startup;
        }
    }
}
