using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResourceProvider {
        public GohResourceProvider GohResourceProvider { get; }

        public GohFactionHumanskinResource? Current { get; private set; }

        public event EventHandler? ResourceUpdated;

        public GohHumanskinResourceProvider(GohResourceProvider ResourceProvider) {
            GohResourceProvider = ResourceProvider;

            GohResourceProvider.ResourceUpdated += OnGohResourceUpdated;
        }

        public void UpdateResource() {
            if (GohResourceProvider.ResourceDictionary != null) {
                Current = new GohFactionHumanskinResource(GohResourceProvider.GetLocationDirectory("ger_humanskin"));
                OnResourceUpdated();
            }
        }

        private void OnGohResourceUpdated(object? sender, EventArgs e) {
            UpdateResource();
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
