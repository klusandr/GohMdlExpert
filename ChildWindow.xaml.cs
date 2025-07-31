using System.ComponentModel;
using System.Windows;

namespace GohMdlExpert {
    /// <summary>
    /// Логика взаимодействия для ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window {
        public Action<CancelEventArgs>? OverrideOnClosing { get; init; }

        public ChildWindow() {
            Owner = App.Current.MainWindow;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e) {
            if (OverrideOnClosing == null) {
                base.OnClosing(e);
            } else {
                OverrideOnClosing(e);
            }
        }
    }
}
