using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Services {
    public static class ErrorReporter {
        private static readonly string ErrorReportURL = "https://forms.gle/juy8Q7PqN155yox99";

        public static void Report() {
            Process.Start(new ProcessStartInfo(ErrorReportURL) { UseShellExecute = true });
        }
    }
}
