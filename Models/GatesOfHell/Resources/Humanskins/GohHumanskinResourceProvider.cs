using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResourceProvider {
        public GohResourceProvider GohResourceProvider { get; }

        public GohFactionHumanskinResource? Current { get; private set; }

        public event EventHandler? HumanskinsResourceUpdate;

        public GohHumanskinResourceProvider(GohResourceProvider gohResourceProvider) {
            GohResourceProvider = gohResourceProvider;

            GohResourceProvider.ResourceUpdate += OnGohResourceUpdate;
        }

        public void Update() {
            if (GohResourceProvider.ResourceDictionary != null) {
                Current = new GohFactionHumanskinResource(GohResourceProvider.GetLocationDirectory("ger_humanskin"));
                OnHumanskinsResourceUpdate();
            }
        }

        private void OnGohResourceUpdate(object? sender, EventArgs e) {
            Update();
        }

        private void OnHumanskinsResourceUpdate() {
            HumanskinsResourceUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}
