using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GohMdlExpert.Services;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl {
        public string GitHubLink = "https://github.com/klusandr";

        public AboutView() {
            InitializeComponent();
            _versionTextBox.Text = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Error!";
        }

        private void ReportButtonClick(object sender, RoutedEventArgs e) {
            Reporter.Report(version: GetType().Assembly.GetName().Version?.ToString());
        }

        private void GirHubHyperlinkHandler(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(GitHubLink) { UseShellExecute = true });
        }
    }
}
