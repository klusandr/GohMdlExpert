using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.Textures;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для TextureLoadTreeItemView.xaml
    /// </summary>
    public partial class TextureLoadTreeItemView : TreeItemView {
        private new TextureLoadTreeItemViewModel ViewModel => (TextureLoadTreeItemViewModel)base.ViewModel;

        public TextureLoadTreeItemView() {
            InitializeComponent();
        }

        private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e) {
            ViewModel.DoubleClickCommand?.Execute(null);
        }
    }
}
