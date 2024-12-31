using GohMdlExpert.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views
{
    /// <summary>
    /// Логика взаимодействия для ModelAdderView.xaml
    /// </summary>
    [BindingViewModel<PlyModelAdderViewModel>]
    [BindingViewModelViaDI]
    public partial class PlyModelAdderView : BaseView {
        public PlyModelAdderView() {
            InitializeComponent();
        }
    }
}
