using GohMdlExpert.ViewModels.ModelsTree;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelTreeItemView.xaml
    /// </summary>
    public partial class ModelsLoadTreeItemView : UserControl {
        public ModelsTreeItemViewModel ViewModel {
            get => (ModelsTreeItemViewModel)DataContext;
            init => DataContext = value;
        }

        public ModelsLoadTreeItemView() {
            InitializeComponent();
        }

        private void IsMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender == this) {
                ViewModel.DoubleClickCommand?.Execute(null);
            }
        }
    }
}
