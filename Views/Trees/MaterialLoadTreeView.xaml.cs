using GohMdlExpert.ViewModels.Trees.Materials;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для MaterialLoadTreeView.xaml
    /// </summary>
    [BindingViewModel<MaterialLoadTreeViewModel>]
    public partial class MaterialLoadTreeView : BaseView {
        public MaterialLoadTreeView() {
            InitializeComponent();
        }
    }
}
