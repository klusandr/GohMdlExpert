using GohMdlExpert.Models.GatesOfHell.Caches;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using Microsoft.Extensions.DependencyInjection;

namespace GohMdlExpert.Models.GatesOfHell {
    public class GohServicesProvider : IServiceProvider {
        private static GohServicesProvider? s_instance;
        private readonly ServiceCollection _services;
        private IServiceProvider? _serviceProvider;

        public static GohServicesProvider Instance => s_instance ??= new();

        public IServiceCollection Services => _services;
        public IServiceProvider ServiceProvider => _serviceProvider ??= _services.BuildServiceProvider();

        private GohServicesProvider() {
            _services = [];
            OnStartup();
        }

        public void OnStartup() {
            _services
                .AddSingleton<ICacheLoader, GohCacheLoader>()
                .AddSingleton<GohCacheProvider>();
        }

        public object? GetService(Type serviceType) {
            return ServiceProvider.GetService(serviceType);
        }
    }
}
