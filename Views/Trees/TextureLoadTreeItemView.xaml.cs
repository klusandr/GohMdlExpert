using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
