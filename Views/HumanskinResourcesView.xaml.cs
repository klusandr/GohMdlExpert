using GohMdlExpert.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для HumanskinResourcesView.xaml
    /// </summary>
    [BindingViewModel<HumanskinResourcesViewModel>]
    [BindingViewModelViaDI]
    public partial class HumanskinResourcesView : BaseView {
        public HumanskinResourcesView() {
            InitializeComponent();
        }
    }
}
