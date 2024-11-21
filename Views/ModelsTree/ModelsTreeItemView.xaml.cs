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
