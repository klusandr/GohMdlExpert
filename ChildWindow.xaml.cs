using System.Windows;

namespace GohMdlExpert {
    /// <summary>
    /// Логика взаимодействия для ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window {
        public ChildWindow() {
            Owner = App.Current.MainWindow;
            InitializeComponent();
        }
    }
}
