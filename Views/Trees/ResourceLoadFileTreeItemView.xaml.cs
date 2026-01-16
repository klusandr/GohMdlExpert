using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.ResourceLoad;
using WpfMvvm.Views.Attributes;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для HumanskinTreeItemView.xaml
    /// </summary>
    [BindingViewModel<ResourceLoadTreeItemViewModel>]
    public partial class ResourceLoadFileTreeItemView : TreeItemView {

        public new ResourceLoadTreeItemViewModel ViewModel => (ResourceLoadTreeItemViewModel)base.ViewModel;

        public ResourceLoadFileTreeItemView() {
            InitializeComponent();
        }

        private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
            if (sender == this) {
                ViewModel.ApproveCommand.Execute(null);
            }
        }
    }
}
