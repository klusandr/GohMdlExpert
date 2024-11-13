using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceFileException : GohException {
        private static readonly string s_message = "Loading Gates of hell resource file error.";

        public GohResourceFileException() : base(GetFullErrorMessage()) { }

        public GohResourceFileException(string message) : base(GetFullErrorMessage(message)) { }

        public GohResourceFileException(string message, Exception inner) : base(GetFullErrorMessage(message), inner) { }

        private static string GetFullErrorMessage(string? message = null) {
            if (message == null) {
                return s_message;
            } else {
                return s_message + " " + message;
            }
        }
    }
}
