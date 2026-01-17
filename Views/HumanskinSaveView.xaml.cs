using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.ViewModels;
using GohMdlExpert.ViewModels.Lists;
using NaturalSort.Extension;
using WpfMvvm.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для HumanskinSaveView.xaml
    /// </summary>
    [BindingViewModel<HumanskinSaveViewModel>]
    [BindingViewModelViaDI]
    public partial class HumanskinSaveView : BaseView {
        private class FileNameComparer : IComparer {
            private static readonly NaturalSortComparer s_natoralComparer = StringComparer.OrdinalIgnoreCase.WithNaturalSort();

            public ListSortDirection SortDirection { get; set; }

            public int Compare(object? x, object? y) {
                if (x == null) { return 1; }
                if (y == null) { return -1; }

                int result = 0;
                var xItem = (ResourceSaveListItemViewModel)x;
                var yItem = (ResourceSaveListItemViewModel)y;


                if (CheckException(xItem, yItem, out result)) {
                    return result;
                }

                var xType = xItem.ResourceElement.GetType();
                var yType = yItem.ResourceElement.GetType();

                if (xType == yType
                    || (xType.IsAssignableTo(typeof(GohResourceFile)) && yType.IsAssignableTo(typeof(GohResourceFile)))
                    || (xType.IsAssignableTo(typeof(GohResourceDirectory)) && yType.IsAssignableTo(typeof(GohResourceDirectory)))
                ) {
                    result = s_natoralComparer.Compare(xItem?.Text, yItem?.Text);
                } else if (xType?.IsAssignableFrom(typeof(GohResourceDirectory)) == true) {
                    result = -1;
                } else if (yType?.IsAssignableFrom(typeof(GohResourceDirectory)) == true) {
                    result = 1;
                }

                if (SortDirection == ListSortDirection.Descending) {
                    result *= -1;
                }

                return result;
            }

            private static bool CheckException(ResourceSaveListItemViewModel? x, ResourceSaveListItemViewModel? y, out int result) {
                result = 0;

                if (x == null || y == null) {
                    return false;
                }

                if (x.Text == "..") {
                    result = -1;
                    return true;
                }

                if (y.Text == "..") {
                    result = 1;
                    return true;
                }

                result = 0;
                return false;
            }
        }

        private FileNameComparer _comparer = new FileNameComparer();

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

        private void HumanskinNameTextBoxKeyDownHandler(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void DataGridSortingHandler(object sender, DataGridSortingEventArgs e) {
            DataGridColumn column = e.Column;

            if (column.SortMemberPath == "Text") {
                e.Handled = true;
                var direction = (column.SortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
                DirectoryItemsDataGridSortByText(direction);
            }
        }

        private void DirectoryItemsDataGridSortByText(ListSortDirection sortDirection = ListSortDirection.Ascending) {
            var column = DirectoryItemsDataGrid.Columns.First();

            var list = (ListCollectionView)CollectionViewSource.GetDefaultView(DirectoryItemsDataGrid.ItemsSource);

            column.SortDirection = _comparer.SortDirection = sortDirection;
            list.CustomSort = _comparer;
        }

        protected override void OnViewModelInitialized(BaseViewModel viewModel) {
            base.OnViewModelInitialized(viewModel);

            ViewModel.CurrentDirectoryItems.CollectionChanged += CurrentDirectoryItemsCollectionChangedHandler;
        }

        private void CurrentDirectoryItemsCollectionChangedHandler(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            DirectoryItemsDataGridSortByText();
        }
    }

    public class SaveParameterVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var item = (ResourceSaveListItemViewModel)value;

            return item.Status == WpfMvvm.Data.Statuses.None ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }


}
