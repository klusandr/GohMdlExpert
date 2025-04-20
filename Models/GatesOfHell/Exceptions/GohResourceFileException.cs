using System.Text;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceFileException : GohException {
        private const string MESSAGE = "Loading GoH resource file {0}error.";

        public string? FileName { get; }
        public GohResourceFile? File { get; }

        protected override byte ExceptionTypeCode => 2;

        public GohResourceFileException(string? message = null, string? fileName = null, Exception? inner = null) : base(GetFullErrorMessage(message, fileName), inner) {
            FileName = fileName;
        }

        public GohResourceFileException(string? message = null, GohResourceFile? file = null, Exception? inner = null) : base(GetFullErrorMessage(message, file?.GetFullPath()), inner) {
            File = file;
        }

        public static GohResourceFileException InvalidExtension(GohResourceFile file, string extension) {
            return new GohResourceFileException($"File has an invalid extension. Expected {extension}", file) { ExceptionCode = 1 };
        }

        public static GohResourceFileException InvalidExtension(string fileName, string extension) {
            return new GohResourceFileException($"File has an invalid extension. Expected {extension}", fileName) { ExceptionCode = 2 };
        }

        public static GohResourceFileException IsNotExists(GohResourceFile file) {
            return new GohResourceFileException($"File is not exists.", file) { ExceptionCode = 3 };
        }

        public static GohResourceFileException IsNotExists(string fileName) {
            return new GohResourceFileException($"File is not exists.", fileName) { ExceptionCode = 4 };
        }

        public static GohResourceFileException InvalidFormat(GohResourceFile file, string formatName) {
            return new GohResourceFileException($"File has an invalid format. Expected {formatName}", file) { ExceptionCode = 5 };
        }

        public static GohResourceFileException InvalidFormat(string fileName, string formatName) {
            return new GohResourceFileException($"File has an invalid format. Expected {formatName}", fileName) { ExceptionCode = 6 };
        }

        public static GohResourceFileException PathIsNull(GohResourceFile file) {
            return new GohResourceFileException($"File path has be null.", file) { ExceptionCode = 7 };
        }

        private static string GetFullErrorMessage(string? message = null, string? fileName = null) {
            var fullMessage = new StringBuilder();

            if (fileName != null) {
                fullMessage.Append(string.Format(MESSAGE, $"\"{fileName}\" "));
            } else {
                fullMessage.Append(MESSAGE);
            }

            if (message != null) {
                fullMessage.Append(' ');
                fullMessage.Append(message);
            }

            return fullMessage.ToString();
        }
    }
}
