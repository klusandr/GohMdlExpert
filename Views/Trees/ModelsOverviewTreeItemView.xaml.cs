using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.OverviewModels;
using WpfMvvm.Views.Attributes;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для ModelsOverviewTreeItemView.xaml
    /// </summary>
    [BindingViewModel<ModelsOverviewTreeItemViewModel>]
    public partial class ModelsOverviewTreeItemView : TreeItemView {
        public ModelsOverviewTreeItemView() {
            InitializeComponent();
        }
    }
}
