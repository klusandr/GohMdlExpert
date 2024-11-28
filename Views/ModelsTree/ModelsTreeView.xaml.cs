using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    public partial class ModelsOverviewTreeView : BaseView {
        public ModelsOverviewTreeView() {
            InitializeComponent();

            DataContext = ViewModelProvider.GetViewModel<ModelsOverviewTreeViewModel>();
        }
    }
}
