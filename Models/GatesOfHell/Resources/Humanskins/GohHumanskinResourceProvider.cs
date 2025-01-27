using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.Collections.ObjectModel;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResourceProvider {
        private readonly GohResourceProvider _resourceProvider;
        private readonly ObservableList<GohFactionHumanskinResource> _humanskinResources;
        private bool _isLoading;
        private int _selectedIndex;

        public GohFactionHumanskinResource Current => _humanskinResources.ElementAtOrDefault(SelectedIndex) ?? throw GohResourcesException.DirectoryNotSpecified();
        public IObservableEnumerable<GohFactionHumanskinResource> HumanskinResources => _humanskinResources;
        public int SelectedIndex {
            get => _selectedIndex;
            set {
                if (value < -1 || value >= _humanskinResources.Count) {
                    throw new ArgumentOutOfRangeException(nameof(SelectedIndex));
                }

                if (value < 0) { value = 0; }

                _selectedIndex = value;
                if (!_isLoading) {
                    OnResourceUpdated();
                }
            }
        }

        public event EventHandler? ResourceUpdated;

        public GohHumanskinResourceProvider(GohResourceProvider ResourceProvider) {
            _resourceProvider = ResourceProvider;
            _resourceProvider.ResourceUpdated += GohResourceUpdated;
            _humanskinResources = [];
        }

        public void UpdateResource() {
            try {
                BeginLoading();
                _humanskinResources.Clear();

                if (_resourceProvider.ResourceDirectory != null) {
                    _humanskinResources.Add(new GohFactionHumanskinResource("German", _resourceProvider.GetLocationDirectory("ger_humanskin"), _resourceProvider));
                    _humanskinResources.Add(new GohFactionHumanskinResource("United states", _resourceProvider.GetLocationDirectory("us_humanskin"), _resourceProvider));
                    _humanskinResources.Add(new GohFactionHumanskinResource("Soviets", _resourceProvider.GetLocationDirectory("sov_humanskin"), _resourceProvider));
                    //_humanskinResources.Add(new GohFactionHumanskinResource("German pak", new Files.GohResourceDirectory("humanskin/[germans]") { Loader = GohServicesProvider.Instance.GetRequiredService<PakDirectoryLoader>()}));
                }
            } finally {
                EndLoading();
            }
        }

        private void BeginLoading() {
            _isLoading = true;
        }

        private void EndLoading() { 
            _isLoading = false;
            OnResourceUpdated();
        }

        private void GohResourceUpdated(object? sender, EventArgs e) {
            UpdateResource();
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
