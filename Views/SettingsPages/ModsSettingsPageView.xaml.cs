using GohMdlExpert.ViewModels.SettingsPages;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для ModsSettingsPageView.xaml
    /// </summary>
    [BindingViewModel<ModsSettingsPageViewModel>(true)]
    public partial class ModsSettingsPageView : SettingsPageView {
        public ModsSettingsPageView() {
            InitializeComponent();
        }
    }
}
