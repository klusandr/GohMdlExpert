using GohMdlExpert.ViewModels.Dialogs;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.Dialogs {
    /// <summary>
    /// Логика взаимодействия для SelectResourceFileDialogView.xaml
    /// </summary>
    [BindingViewModel<SelectResourceFileDialogViewModel>]
    [BindingViewModelViaDI]
    public partial class SelectResourceFileDialogView : BaseView {
        public new SelectResourceFileDialogViewModel ViewModel => (SelectResourceFileDialogViewModel)base.ViewModel;
        public SelectResourceFileDialogView() {
            InitializeComponent();
        }
    }
}
