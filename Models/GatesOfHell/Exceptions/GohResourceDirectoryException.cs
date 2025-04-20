using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceDirectoryException : GohResourcesException {
        private const string MESSAGE = "Dictionary load {0}error.{1}";

        protected override byte ExceptionTypeCode => 6;

        public GohResourceDirectoryException(string? message = null, GohResourceDirectory? directory = null, Exception? inner = null) : base(GetFullErrorMessage(message, directory), inner) { }

        public static GohResourceDirectoryException PathIsNull(GohResourceDirectory directory) {
            return new GohResourceDirectoryException($"Path has be null.", directory) { ExceptionCode = 1 };
        }

        private static string GetFullErrorMessage(string? message, GohResourceDirectory? directory) {
            return string.Format(MESSAGE,
                directory != null ? directory.Name + ' ' : string.Empty,
                message != null ? ' ' + message : string.Empty);
        }
    }
}
