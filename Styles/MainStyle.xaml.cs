using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GohMdlExpert.Styles {
    public partial class MainStyle : ResourceDictionary {
        private void LoadedHandler(object sender, RoutedEventArgs e) {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Activated += WindowActivatedHandler;
        }

        private void WindowActivatedHandler(object? sender, EventArgs e) {
            ((UIElement)sender!).InvalidateMeasure();
        }
    }
}
