using System.Windows.Controls;
using GohMdlExpert.ViewModels.ModelsTree.LoadModels;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.ModelsTree {
    /// <summary>
    /// Логика взаимодействия для ModelsTreeView.xaml
    /// </summary>
    [BindingViewModel<ModelsLoadTreeViewModel>]
    [BindingViewModelViaDI]
    public partial class ModelsLoadTreeView : BaseView {
        public ModelsLoadTreeView() {
            InitializeComponent();
        }
    }
}
