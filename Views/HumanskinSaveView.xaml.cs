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
using GohMdlExpert.ViewModels;
using GohMdlExpert.ViewModels.Lists;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для HumanskinSaveView.xaml
    /// </summary>
    [BindingViewModel<HumanskinSaveViewModel>]
    [BindingViewModelViaDI]
    public partial class HumanskinSaveView : BaseView {

        public new HumanskinSaveViewModel ViewModel => (HumanskinSaveViewModel)base.ViewModel;

        public HumanskinSaveView() {
            InitializeComponent();
        }

        private void ListViewItemMouseLeftButtonDownHendler(object sender, MouseButtonEventArgs e) {
            if (sender is FrameworkElement { DataContext: ResourceLoadListItemViewModel listItemViewNodel }) {
                listItemViewNodel.ApproveCommand.Execute(null);
            }
        }

        private void DataGridLoadingRowHandler(object sender, DataGridRowEventArgs e) {
            e.Row.MouseDoubleClick += ListViewItemMouseLeftButtonDownHendler;
        }
    }
}
