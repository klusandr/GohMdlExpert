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

namespace GohMdlExpert.Views.Dialogs {
    /// <summary>
    /// Логика взаимодействия для TextRequestView.xaml
    /// </summary>
    public partial class RequestTextDialogView : UserControl {
        public RequestTextDialogView() {
            InitializeComponent();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                OkButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
