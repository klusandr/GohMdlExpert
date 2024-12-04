using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    public partial class ModelsOverviewTreeView : BaseView {

        private ModelsOverviewTreeViewModel OverviewTreeViewModel => (ModelsOverviewTreeViewModel)DataContext;

        public ModelsOverviewTreeView() {
            InitializeComponent();

            DataContext = ViewModelProvider.GetViewModel<ModelsOverviewTreeViewModel>();
            _tree.SelectedItemChanged += OverviewTreeViewModel.SelectedItemChanged;
        }
    }
}
