using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.Collections.ObjectModel;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResourceProvider {
        private readonly GohResourceProvider _resourceProvider;
        private readonly GohCacheProvider _cacheProvider;
        private IGohHumanskinResource? _resource;

        public IGohHumanskinResource Resource { get => _resource ?? throw GohResourcesException.IsNotLoad(); private set => _resource = value; }

        public event EventHandler? ResourceUpdated;

        public GohHumanskinResourceProvider(GohResourceProvider ResourceProvider) {
            _resourceProvider = ResourceProvider;
            _cacheProvider = GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>();
            _resourceProvider.ResourceFullLoaded += GohResourceFullLoadedHandler;
        }

        public void UpdateResource() {
            Resource = new GohHumanskinResource(_resourceProvider.GetLocationDirectory(GohResourceLocations.Humanskin), _resourceProvider, _cacheProvider);
            OnResourceUpdated();
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void GohResourceFullLoadedHandler(object? sender, EventArgs e) {
            UpdateResource();
        }
    }
}
