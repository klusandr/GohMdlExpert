using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.LoadModels;
using WpfMvvm.Views.Attributes;
using WpfMvvm.Views.Controls;


namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для ModelTreeItemView.xaml
    /// </summary>
    [BindingViewModel<ModelsLoadTreeItemViewModel>]
    public partial class ModelsLoadTreeItemView : TreeItemView {

        public new ModelsLoadTreeItemViewModel ViewModel => (ModelsLoadTreeItemViewModel)base.ViewModel;

        public ModelsLoadTreeItemView() {
            InitializeComponent();
        }

        private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
            if (sender == this) {
                ViewModel.ApproveCommand.Execute(null);
            }
        }
    }
}
