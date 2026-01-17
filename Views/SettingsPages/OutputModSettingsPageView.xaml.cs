using GohMdlExpert.ViewModels.SettingsPages;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для OutputModSettingsPageView.xaml
    /// </summary>
    [BindingViewModel<OutputModSettingsPageViewModel>(true)]
    public partial class OutputModSettingsPageView : SettingsPageView {
        public OutputModSettingsPageView() {
            InitializeComponent();
        }
    }
}
