using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
