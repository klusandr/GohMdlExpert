using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using WpfMvvm.Collections.ObjectModel;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResourceProvider {
        private readonly GohResourceProvider _resourceProvider;
        private readonly ObservableList<GohFactionHumanskinResource> _humanskinResources;
        private int _selectedIndex;

        public GohFactionHumanskinResource Current => _humanskinResources.ElementAtOrDefault(SelectedIndex) ?? throw GohResourcesException.DirectoryNotSpecified(); 
        public IObservableEnumerable<GohFactionHumanskinResource> HumanskinResources => _humanskinResources;
        public int SelectedIndex {
            get => _selectedIndex;
            set {
                if (value < 0 || value >= _humanskinResources.Count) {
                    throw new ArgumentOutOfRangeException(nameof(SelectedIndex));
                }

                _selectedIndex = value;
                OnResourceUpdated();
            }
        }

        public event EventHandler? ResourceUpdated;

        public GohHumanskinResourceProvider(GohResourceProvider ResourceProvider) {
            _resourceProvider = ResourceProvider;
            _resourceProvider.ResourceUpdated += GohResourceUpdated;
            _humanskinResources = [];
        }

        public void UpdateResource() {
            if (_resourceProvider.ResourceDictionary != null) {
                _humanskinResources.Clear();
                _humanskinResources.Add(new GohFactionHumanskinResource("German", _resourceProvider.GetLocationDirectory("ger_humanskin")));
                _humanskinResources.Add(new GohFactionHumanskinResource("United states", _resourceProvider.GetLocationDirectory("us_humanskin")));

                OnResourceUpdated();
            }
        }

        private void GohResourceUpdated(object? sender, EventArgs e) {
            UpdateResource();
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
