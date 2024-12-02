using GohMdlExpert.ViewModels.ModelsTree;
using System.Windows.Controls;
using System.Windows.Input;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelTreeItemView.xaml
    /// </summary>
    public partial class ModelsTreeItemView : UserControl {
        public ModelsTreeItemViewModel ViewModel {
            get => (ModelsTreeItemViewModel)DataContext;
            init => DataContext = value;
        }

        public ModelsTreeItemView() {
            InitializeComponent();
        }

        private void IsMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender == this) {
                ViewModel.DoubleClickCommand?.Execute(null);
            }
        }

        private void ModelsTreeItemViewExpanded(object sender, System.Windows.RoutedEventArgs e) {
            ViewModel.IsExpended = true;
        }

        private void ModelsTreeItemViewCollapsed(object sender, System.Windows.RoutedEventArgs e) {
            ViewModel.IsExpended = false;
        }
    }
}
