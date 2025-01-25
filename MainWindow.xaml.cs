using GohMdlExpert.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMvvm.DependencyInjection;
using WpfMvvm.Diagnostics;
using WpfMvvm.ViewModels;

namespace GohMdlExpert {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public ApplicationViewModel ViewModel => (ApplicationViewModel)DataContext;

        public MainWindow() {
            InitializeComponent();

            DataContext = App.Current.ServiceProvider.GetRequiredService<ApplicationViewModel>();
        }

        private void MenuItemExitClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}