using GohMdlExpert.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для MaterialLoadView.xaml
    /// </summary>
    [BindingViewModel<MaterialLoadViewModel>]
    [BindingViewModelViaDI]
    public partial class MaterialLoadView : BaseView {

        public new MaterialLoadViewModel ViewModel => (MaterialLoadViewModel)base.ViewModel;

        public MaterialLoadView() {
            InitializeComponent();
        }
    }
}
