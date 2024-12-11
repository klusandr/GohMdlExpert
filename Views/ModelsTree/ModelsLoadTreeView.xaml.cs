using System.Windows.Controls;
using GohMdlExpert.ViewModels.ModelsTree.LoadModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.ModelsTree
{
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    public partial class ModelsLoadTreeView : BaseView {
        public ModelsLoadTreeView() {
            InitializeComponent();
            ViewModel = ViewModelProvider.GetRequiredViewModel<ModelsLoadTreeViewModel>();
        }
    }
}
