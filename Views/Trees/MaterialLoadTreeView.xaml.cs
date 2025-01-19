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
