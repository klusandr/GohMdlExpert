using System.Windows.Controls;
using GohMdlExpert.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для PlyModelLodList.xaml
    /// </summary>
    [BindingViewModel<PlyModelLodListViewModel>]
    public partial class PlyModelLodListView : BaseView {
        private ListViewItem? _mainLodListItemView;

        public PlyModelLodListView() {
            InitializeComponent();
        }

        private void MainLodListViewItemMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            ListViewItem listViewItem;

            if (sender is TextBlock textBlock) {
                listViewItem = (ListViewItem)textBlock.Parent;
            } else {
                listViewItem = (ListViewItem)sender;
            }

            _mainLodListItemView = listViewItem;

            LodListView.SelectedItem = null;
            _mainLodListItemView.IsSelected = true;
        }

        private void LodListViewSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (_mainLodListItemView?.IsSelected == true) {
                _mainLodListItemView.IsSelected = false;
            }
        }
    }
}
