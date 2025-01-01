using System.ComponentModel;
using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;
using System.Windows;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    [BindingViewModel<ModelsOverviewTreeViewModel>]
    public partial class ModelsOverviewTreeView : BaseView {

        public new ModelsOverviewTreeViewModel ViewModel => (ModelsOverviewTreeViewModel)base.ViewModel;

        public ModelsOverviewTreeView() {
            InitializeComponent();

            DataContextChanged += (_, _) => {
                ViewModel.PropertyChangeHandler.AddHandler(nameof(ViewModel.SelectedItem), SelectedItemChangeHandler);
            };
        }

        private void SelectedItemChangeHandler(object? sender, PropertyChangedEventArgs e) {
            switch (ViewModel.SelectedItem) {
                case ModelsOverviewTreePlyViewModel:
                    _lodList.Visibility = Visibility.Visible;
                    _materialList.Visibility = Visibility.Collapsed;
                    break;
                case ModelsOverviewTreeMtlViewModel:
                    _materialList.Visibility = Visibility.Visible;
                    _lodList.Visibility = Visibility.Collapsed;
                    break;
                default:
                    _materialList.Visibility = Visibility.Collapsed;
                    _lodList.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
