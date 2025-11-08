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
using GohMdlExpert.ViewModels.Trees.Humanskins;
using GohMdlExpert.ViewModels.Trees.LoadModels;
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
