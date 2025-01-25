using GohMdlExpert.ViewModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для PlyModelLodList.xaml
    /// </summary>
    [BindingViewModel<PlyModelLodListViewModel>]
    public partial class PlyModelLodListView : BaseView {
        public PlyModelLodListView() {
            InitializeComponent();
        }
    }
}
