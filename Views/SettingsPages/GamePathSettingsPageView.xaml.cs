using GohMdlExpert.ViewModels.SettingsPages;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для GamePathSettingsPageView.xaml
    /// </summary>
    [BindingViewModel<GamePathSettingsPageViewModel>(true)]
    public partial class GamePathSettingsPageView : SettingsPageView {
        public GamePathSettingsPageView() {
            InitializeComponent();
        }
    }
}
