using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class GohOutputModProvider {
        private OutputModResource? _mod;

        public OutputModResource Mod {
            get => _mod ?? throw GohResourceSaveException.OutputModIsNotSet();
            set {
                _mod = value;
                ModUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool ModIsLoaded => _mod != null;

        public event EventHandler? ModUpdate;

        public GohOutputModProvider() { }

        public void ClearMod() {
            _mod = null;
            ModUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}
