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
using GohMdlExpert.ViewModels.SettingsPages;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для OutputModSettingsPageView.xaml
    /// </summary>
    [BindingViewModel<OutputModSettingsPageViewModel>(true)]
    public partial class OutputModSettingsPageView : SettingsPageView {
        public OutputModSettingsPageView() {
            InitializeComponent();
        }
    }
}
