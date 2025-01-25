using System.Windows;
using GohMdlExpert.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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