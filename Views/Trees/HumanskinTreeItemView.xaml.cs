using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.Humanskins;
using WpfMvvm.Views.Attributes;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для HumanskinTreeItemView.xaml
    /// </summary>
    [BindingViewModel<HumanskinTreeItemViewModel>]
    public partial class HumanskinTreeItemView : TreeItemView {

        public new HumanskinTreeItemViewModel ViewModel => (HumanskinTreeItemViewModel)base.ViewModel;

        public HumanskinTreeItemView() {
            InitializeComponent();
        }

        private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
            if (sender == this) {
                ViewModel.ApproveCommand.Execute(null);
            }
        }
    }
}
