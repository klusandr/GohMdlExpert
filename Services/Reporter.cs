using System.Diagnostics;

namespace GohMdlExpert.Services {
    public enum ReportType {
        Error,
        Defect,
        Pay,
        Offer,
        Wish
    }

    public static class Reporter {
        private static readonly string s_reportURL = "https://forms.gle/juy8Q7PqN155yox99";
        private static readonly string s_fillReportURL = "https://docs.google.com/forms/d/e/1FAIpQLSf71lGTFWFvJ2kdImoov2nW6YFqWRU3UFgmaBQpPdL_WSzS_g/viewform?usp=pp_url";

        private static readonly string s_reportTypeUrlField = "&entry.362427890=";
        private static readonly string s_versionUrlField = "&entry.1551268863=";
        private static readonly string s_errorMassageUrlField = "&entry.1278925591=";

        public static void Report(ReportType? reportType = null, string? version = null, string? errorMassage = null) {
            string url;

            if (reportType != null || version != null || errorMassage != null) {
                url = s_fillReportURL
                    + (reportType != null ? s_reportTypeUrlField + reportType.ToString() : string.Empty)
                    + (version != null ? s_versionUrlField + version : string.Empty)
                    + (errorMassage != null ? s_errorMassageUrlField + Uri.EscapeDataString(errorMassage) : string.Empty);
            } else {
                url = s_reportURL;
            }

            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
