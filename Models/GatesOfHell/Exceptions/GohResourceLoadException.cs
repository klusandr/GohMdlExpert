using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceLoadException : GohException {
        private static readonly string s_message = "Loading Gates of hell resources error.";

        protected override byte ExceptionTypeCode => 1;

        public GohResourceLoadException(string? message = null, Exception? inner = null) : base(GetFullErrorMessage(message), inner) { }

        public static GohResourceLoadException IsNotGohGameDirectory(string path) {
            return new GohResourceLoadException($"\"{path}\" is not GoH game directory.") { ExceptionCode = 1 };
        }

        public static GohResourceLoadException IsNotGohResource(string path) {
            return new GohResourceLoadException($"\"{path}\" is not GoH resource directory.") { ExceptionCode = 2 };
        }

        public static GohResourceLoadException IsNotLoad() {
            return new GohResourceLoadException($"Resource is not load.") { ExceptionCode = 3 };
        }

        public static GohResourceLoadException LocationNotDefined(string location) {
            return new GohResourceLoadException($"Resource location \"{location}\" is not defined.") { ExceptionCode = 4 };
        }

        internal static GohResourceLoadException AggregateLocationNotDefined(string aggregateLocations) {
            return new GohResourceLoadException($"Resource aggregate location \"{aggregateLocations}\" is not defined.") { ExceptionCode = 5 };
        }

        public static GohResourceLoadException LocationNotFound(string location, string path) {
            return new GohResourceLoadException($"Resource location \"{location}\" on \"{path}\" not found.") { ExceptionCode = 6 };
        }

        public static GohResourceLoadException PathIsNull(GohResourceElement resourceElement) {
            return new GohResourceLoadException($"Resource element \"{resourceElement.Name}\" path has be null.") { ExceptionCode = 7 };
        }

        public static GohResourceLoadException ElementNotInResource(GohResourceElement resourceElement) {
            return new GohResourceLoadException($"Resource element \"{resourceElement.Name}\" is not in resource folder or subfolders.") { ExceptionCode = 8 };
        }

        public static GohResourceLoadException ElementNotInResource(string resourceElementName) {
            return new GohResourceLoadException($"Resource element \"{resourceElementName}\" is not in resource folder or subfolders.") { ExceptionCode = 9 };
        }

        public static GohResourceLoadException ResourcesPathNotHaveOneRoot(string firstResource, string secondResource) {
            return new GohResourceLoadException(string.Format("Resources haven't one path root. First resource: \"{0}\". Second resource \"{1}\"", firstResource, secondResource)) { ExceptionCode = 11 };
        }

        private static string GetFullErrorMessage(string? message = null) {
            if (message == null) {
                return s_message;
            } else {
                return s_message + " " + message;
            }
        }
    }
}
