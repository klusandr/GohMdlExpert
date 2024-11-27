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

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender == this) {
                ViewModel.DoubleClickCommand?.Execute(null);
            }
        }
    }
}
