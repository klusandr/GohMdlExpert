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
using System.Windows.Shapes;
using GohMdlExpert.Services;
using WpfMvvm.Views.Dialogs;

namespace GohMdlExpert.Views.Dialogs {
    /// <summary>
    /// Логика взаимодействия для UserErrorDialogWindow.xaml
    /// </summary>
    public partial class UserErrorDialogWindow : Window {
        public new QuestionResult? DialogResult { get; private set; }

        public UserErrorDialogWindow(Window owner, string message, Exception? exception = null, string? title = null) {
            InitializeComponent(); 
            
            Title = title ?? "Error!";
            Owner = owner;
            Messagse.Text = message;

            if (exception != null) {
                if (!string.IsNullOrEmpty(message)) {
                    Messagse.Text = "\n\n";
                }

                Messagse.Text +=
                    $"Error message: {exception.Message}\n\n" +
                    $"Code: 0x{exception.HResult:x}\n\n" +
                    $"Stack trace: {exception.StackTrace}";
            }


            if (message.Length > 300) {
                Messagse.FontSize = 12;
            }
        }

        public UserErrorDialogWindow(string message, Exception? exception = null, string? title = null) : this(Application.Current.MainWindow, message, exception, title) { }

        public new QuestionResult? ShowDialog() {
            base.ShowDialog();
            return DialogResult;
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e) {
            DialogResult = QuestionResult.OK;
            Close();
        }

        private void ButtonReportClick(object sender, RoutedEventArgs e) {
            Reporter.Report(ReportType.Error, GetType().Assembly.GetName().Version?.ToString(), Messagse.Text);
        }
    }
}
