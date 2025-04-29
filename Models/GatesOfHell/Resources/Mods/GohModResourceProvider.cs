using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods
{
    public class GohModResourceProvider {
        private List<ModResource> _mods;

        public IEnumerable<ModResource> Mods { get => _mods; }

        public GohModResourceProvider() {
            _mods = [];
        }

        public void AddMod(ModResource mod) {
            mod.Load();
            _mods.Add(mod);
        }

        public void RemoveMod(ModResource mod) { 
            _mods.Remove(mod);
        }
    }
}
