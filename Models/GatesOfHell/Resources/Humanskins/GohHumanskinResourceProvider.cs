using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResourceProvider {
        private readonly GohResourceProvider _resourceProvider;

        public GohFactionHumanskinResource? Current { get; private set; }
        public GohFactionHumanskinResource OutputResource;

        public event EventHandler? ResourceUpdated;

        public GohHumanskinResourceProvider(GohResourceProvider ResourceProvider) {
            _resourceProvider = ResourceProvider;
            _resourceProvider.ResourceUpdated += GohResourceUpdated;
        }

        public void UpdateResource() {
            if (_resourceProvider.ResourceDictionary != null) {
                Current = new GohFactionHumanskinResource("German", _resourceProvider.GetLocationDirectory("ger_humanskin"));
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
