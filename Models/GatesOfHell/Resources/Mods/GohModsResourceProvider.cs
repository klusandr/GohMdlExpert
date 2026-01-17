namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class GohModsResourceProvider {
        private readonly List<ModResource> _mods;

        public IEnumerable<ModResource> Mods { get => _mods; }

        public GohModsResourceProvider() {
            _mods = [];
        }

        public void AddMod(ModResource mod) {
            mod.Load();
            _mods.Add(mod);
        }

        public void RemoveMod(ModResource mod) {
            _mods.Remove(mod);
        }

        public void ClearMods() {
            _mods.Clear();
        }
    }
}
