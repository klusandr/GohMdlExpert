using GohMdlExpert.Properties;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public abstract class SettingsPageViewModel : BaseViewModel {
        protected Settings Settings { get; } = Settings.Default;
        public abstract string Name { get; }

        public event EventHandler? SettingsApproved;

        public virtual void LoadSettings() { }

        public virtual void SaveSettings() {
            Settings.Save();
        }

        protected void ApproveSettings() {
            SettingsApproved?.Invoke(this, EventArgs.Empty);
        }
    }
}
