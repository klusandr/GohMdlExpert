using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvm.DependencyInjection;
using WpfMvvm.ViewModels;

namespace GohMdlExpert {
    public static class AppStartup {
        public static void Startup(object? sender, EventArgs e) {
            AppDependencyInjection.Instance.Startup += ServicesStartup;
            ViewModelsProvider.Initialize(AppDependencyInjection.Instance);
            ViewModelsProvider.Startup += ViewModelsStartup.Startup;
            AppDependencyInjection.Instance.OnStartup();
        }

        public static void ServicesStartup(object? sender, DependencyInjectionSratupArgs e) {
            e.Services.AddSingleton<GohResourceLocations>();
            e.Services.AddSingleton<GohResourceProvider>();
            e.Services.AddSingleton<GohHumanskinResourceProvider>();
            e.Services.AddSingleton<GohTextureProvider>();
        }
    }
}
