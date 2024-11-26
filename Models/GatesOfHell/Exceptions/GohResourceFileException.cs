using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceFileException : GohException {
        private const string MESSAGE = "Loading Gates of hell resource file {0}error.";

        public string? FileName { get; init; }

        public GohResourceFileException(string? fileName = null) : base(GetFullErrorMessage(fileName: fileName)) {
            FileName = fileName;
        }

        public GohResourceFileException(string message, string? fileName = null) : base(GetFullErrorMessage(message, fileName)) { }

        public GohResourceFileException(string message, Exception inner, string? fileName = null) : base(GetFullErrorMessage(message, fileName), inner) { }

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
