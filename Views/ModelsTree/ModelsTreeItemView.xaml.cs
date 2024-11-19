using GohMdlExpert.ViewModels.ModelsTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelTreeItemView.xaml
    /// </summary>
    public partial class ModelsTreeItemView : TreeViewItem {
        public ModelsTreeItemViewModel ViewModel => (ModelsTreeItemViewModel)DataContext;

        public ModelsTreeItemView(ModelsTreeItemViewModel viewModel) {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender == this && IsSelected) {
                ViewModel.DoubleClickCommand?.Execute(null);
            }
        }

        private void OnExpanded(object sender, RoutedEventArgs e) {
            if (sender == this && IsSelected) {
                ViewModel.ExpandedCommand?.Execute(null);
            }
        }
    }
}
