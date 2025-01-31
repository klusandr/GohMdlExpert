using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Caches;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.DependencyInjection;

namespace GohMdlExpert.Models.GatesOfHell
{
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
                .AddSingleton<IFileLoader, DefaultFileLoader>()
                .AddSingleton<IDirectoryLoader, DefaultDirectoryLoader>()
                .AddSingleton<ICacheLoader, GohCacheLoader>()
                .AddSingleton<GohCacheProvider>();
        }

        public object? GetService(Type serviceType) {
            return ServiceProvider.GetService(serviceType);
        }
    }
}
