using System.Windows.Controls;
using GohMdlExpert.ViewModels.ModelsTree.LoadModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.ModelsTree
{
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    public partial class LoadModelsTreeView : BaseView {
        public LoadModelsTreeView() {
            InitializeComponent();
            ViewModel = ViewModelProvider.GetRequiredViewModel<ModelsLoadTreeViewModel>();
        }

        private void HierarchicalDataTemplate_Expanded(object sender, System.Windows.RoutedEventArgs e) {

        }
    }
}
