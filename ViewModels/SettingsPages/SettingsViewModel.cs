using WpfMvvm.DependencyInjection;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public class SettingsViewModel : BaseViewModel {
        private readonly HashSet<SettingsPageViewModel> _pages;
        private SettingsPageViewModel? _selectedPage;

        public IEnumerable<SettingsPageViewModel> Pages => _pages;

        public GamePathSettingsPageViewModel GamePathSettingsPage => GetPage<GamePathSettingsPageViewModel>()!;
        public ModsSettingsPageViewModel ModsSettingsPage => GetPage<ModsSettingsPageViewModel>()!;

        public SettingsPageViewModel? SelectedPage {
            get => _selectedPage;
            set {
                _selectedPage = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler? SettingsApproved;

        public SettingsViewModel() {
            var serviceProvider = App.Current.ServiceProvider;

            _pages = [
                serviceProvider.CreateInstance<GamePathSettingsPageViewModel>(),
                serviceProvider.CreateInstance<ModsSettingsPageViewModel>(),
                serviceProvider.CreateInstance<OutputModSettingsPageViewModel>()
            ];


            foreach (var page in _pages) {
                page.SettingsApproved += PageSettingsApprovedHandler;
            }
        }

        private void PageSettingsApprovedHandler(object? sender, EventArgs e) {
            SettingsApproved?.Invoke(this, EventArgs.Empty);
        }

        public SettingsPageViewModel? GetPage(Type type) {
            return Pages.FirstOrDefault(p => p.GetType() == type);
        }

        public T? GetPage<T>() where T : SettingsPageViewModel {
            return (T?)Pages.FirstOrDefault(p => p is T);
        }

        public void LoadSettings() {
            foreach (var page in _pages) {
                page.LoadSettings();
            }
        }

        public void SaveSettings() {
            foreach (var page in _pages) {
                page.SaveSettings();
            }
        }
    }
}
