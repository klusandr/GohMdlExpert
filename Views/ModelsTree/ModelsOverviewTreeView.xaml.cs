using GohMdlExpert.ViewModels.ModelsTree.OverviewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    [BindingViewModel<ModelsOverviewTreeViewModel>]
    public partial class ModelsOverviewTreeView : BaseView {
        public ModelsOverviewTreeView() {
            InitializeComponent();
        }
    }
}
