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
using WpfMvvm.ViewModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для SettingsPageView.xaml
    /// </summary>
    public abstract class SettingsPageView : BaseView {

        public string? PageName => (ViewModel as SettingsPageViewModel)?.Name;

        public SettingsPageView() { }
    }
}
