using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using GohMdlExpert.ViewModels.Dialogs;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.Dialogs {
    /// <summary>
    /// Логика взаимодействия для SelectResourceFileDialogView.xaml
    /// </summary>
    [BindingViewModel<SelectResourceFileDialogViewModel>]
    [BindingViewModelViaDI]
    public partial class SelectResourceFileDialogView : BaseView {
        public new SelectResourceFileDialogViewModel ViewModel => (SelectResourceFileDialogViewModel)base.ViewModel;
        public SelectResourceFileDialogView() {
            InitializeComponent();
        }
    }
}
