using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.DependencyInjection;

namespace GohMdlExpert.ViewModels {
    public static class ViewModelsStartup {
        public static void Startup(object? sender, DependencyInjectionStatupArgs e) {
            e.Services.AddSingleton<ApplicationViewModel>();
            e.Services.AddSingleton<HumanskinMdlOverviewViewModel>();
            e.Services.AddSingleton<HumanskinMdlGeneratorViewModel>();
            e.Services.AddSingleton<HumanskinResourcesViewModel>();
            e.Services.AddSingleton<MaterialLoadViewModel>();
        }
    }
}
