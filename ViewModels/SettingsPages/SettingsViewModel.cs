using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvm.DependencyInjection;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public class SettingsViewModel : BaseViewModel {
        private readonly HashSet<SettingsPageViewModel> _pages;
        private SettingsPageViewModel? _selectedPage;

        public IEnumerable<SettingsPageViewModel> Pages => _pages;
        public GamePathSettingsPageViewModel GamePathSettingsPage => GetPage<GamePathSettingsPageViewModel>()!;

        public SettingsPageViewModel? SelectedPage {
            get => _selectedPage;
            set {
                _selectedPage = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel() {
            var serviceProvider = App.Current.ServiceProvider;

            _pages = [
                serviceProvider.CreateInstance<GamePathSettingsPageViewModel>()
            ];
        }

        public SettingsPageViewModel? GetPage(Type type) {
            return Pages.FirstOrDefault(p => p.GetType() == type);
        }

        public T? GetPage<T>() where T: SettingsPageViewModel {
            return (T?)Pages.FirstOrDefault(p => p is T);
        }
    }
}
