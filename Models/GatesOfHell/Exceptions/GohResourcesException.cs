using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourcesException : GohException {
        private static readonly string s_message = "Loading Gates of hell resources error.";

        protected override byte ExceptionTypeCode => 1;

        public GohResourcesException(string? message = null, Exception? inner = null) : base(GetFullErrorMessage(message), inner) { }

        public static GohResourcesException IsNotGohGameDirectory(string path) {
            return new GohResourcesException($"\"{path}\" is not GoH game directory.") { ExceptionCode = 1 };
        }

        public static GohResourcesException IsNotGohResource(string path) {
            return new GohResourcesException($"\"{path}\" is not GoH resource directory.") { ExceptionCode = 2 };
        }

        public static GohResourcesException IsNotLoad() {
            return new GohResourcesException($"Resource is not load.") { ExceptionCode = 3 };
        }

        public static GohResourcesException LocationNotDefined(string location) {
            return new GohResourcesException($"Resource location \"{location}\" is not defined.") { ExceptionCode = 4 };
        }

        internal static GohResourcesException AggregateLocationNotDefined(string aggregateLocations) {
            return new GohResourcesException($"Resource aggregate location \"{aggregateLocations}\" is not defined.") { ExceptionCode = 5 };
        }

        public static GohResourcesException LocationNotFound(string location, string path) {
            return new GohResourcesException($"Resource location \"{location}\" on \"{path}\" not found.") { ExceptionCode = 6 };
        }

        public static GohResourcesException PathIsNull(GohResourceElement resourceElement) {
            return new GohResourcesException($"Resource element \"{resourceElement.Name}\" path has be null.") { ExceptionCode = 7 };
        }

        public static GohResourcesException ElementNotInResource(GohResourceElement resourceElement) {
            return new GohResourcesException($"Resource element \"{resourceElement.Name}\" is not in resource folder or subfolders.") { ExceptionCode = 8 };
        }

        public static GohResourcesException ElementNotInResource(string resourceElementName) {
            return new GohResourcesException($"Resource element \"{resourceElementName}\" is not in resource folder or subfolders.") { ExceptionCode = 9 };
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
