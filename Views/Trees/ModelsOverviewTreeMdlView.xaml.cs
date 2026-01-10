using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.OverviewModels;
using WpfMvvm.Controls;
using WpfMvvm.Views.Attributes;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для ModelsOverviewTreeItemView.xaml
    /// </summary>
    [BindingViewModel<ModelsOverviewTreeMdlViewModel>]
    public partial class ModelsOverviewTreeMdlView : TreeItemView {
        public new ModelsOverviewTreeMdlViewModel ViewModel => (ModelsOverviewTreeMdlViewModel)base.ViewModel;

        public ModelsOverviewTreeMdlView() {
            InitializeComponent();
            DataContextChanged += DataContextChangedHandler;
            EditebleTextBlock.MouseLeftButtonDown += EditebleTextBlockMouseLeftButtonDownHandler;
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.Key) {
                case Key.F2: ViewModel.IsEdit = true; break;
            }

            base.OnKeyDown(e);
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e) {
            ViewModel.PropertyChangeHandler.AddHandler(nameof(ViewModel.IsEdit), (_, _) => EditebleTextBlock.IsEditable = ViewModel.IsEdit);
        }

        private void EditebleTextBlockMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e) {
            ViewModel.IsSelected = true;
        }
    }
}
