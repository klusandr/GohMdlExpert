using GohMdlExpert.ViewModels.SettingsPages;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для SettingsPageView.xaml
    /// </summary>
    public abstract class SettingsPageView : BaseView {

        public string? PageName => (ViewModel as SettingsPageViewModel)?.Name;

        public SettingsPageView() { }
    }
}
