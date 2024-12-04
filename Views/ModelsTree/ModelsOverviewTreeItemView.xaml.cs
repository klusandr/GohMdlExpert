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
using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelsOverviewTreeItemView.xaml
    /// </summary>
    public partial class ModelsOverviewTreeItemView : UserControl {

        public ModelsOverviewTreeItemViewModel? ViewModel => DataContext as ModelsOverviewTreeItemViewModel;

        public ModelsOverviewTreeItemView() {
            InitializeComponent();
        }

        private void MouseLeftClick(object sender, MouseButtonEventArgs e) {
            ViewModel?.MouseLeftClickCommand?.Execute(null);
        }
    }
}
