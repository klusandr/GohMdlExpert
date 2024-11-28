using GohMdlExpert.ViewModels.ModelsTree;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.ModelsTree
{
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    public partial class LoadModelsTreeView : BaseView {
        public LoadModelsTreeView() {
            InitializeComponent();

            ViewModel = ViewModelProvider.GetRequiredViewModel<LoadModelsTreeViewModel>();
        }
    }
}
