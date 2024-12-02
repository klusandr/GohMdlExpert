using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceFileException : GohException {
        private const string MESSAGE = "Loading GoH resource file {0}error.";

        public string? FileName { get; }
        public GohResourceFile? File { get; }

        public GohResourceFileException(string? message = null, string? fileName = null, Exception? inner = null) : base(GetFullErrorMessage(message, fileName), inner) {
            FileName = fileName;
        }

        public GohResourceFileException(string? message = null, GohResourceFile? file = null, Exception? inner = null) : base(GetFullErrorMessage(message, file?.GetFullPath()), inner) {
            File = file;
        }

        public static GohResourceFileException InvalidExtension(GohResourceFile file, string extension) {
            return new GohResourceFileException($"File has an invalid extension. Expected {extension}", file);
        }

        public static GohResourceFileException InvalidExtension(string fileName, string extension) {
            return new GohResourceFileException($"File has an invalid extension. Expected {extension}", fileName);
        }

        public static GohResourceFileException IsNotExists(GohResourceFile file) {
            return new GohResourceFileException($"File is not exists.", file);
        }

        public static GohResourceFileException IsNotExists(string fileName) {
            return new GohResourceFileException($"File is not exists.", fileName);
        }

        public static GohResourceFileException InvalidFormat(GohResourceFile file, string formatName) {
            return new GohResourceFileException($"File has an invalid format. Expected {formatName}", file);
        }

        public static GohResourceFileException InvalidFormat(string fileName, string formatName) {
            return new GohResourceFileException($"File has an invalid format. Expected {formatName}", fileName);
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
