using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
	public class PlyModelException : GohException {
        private const string MESSAGE = "Gates of hell ply model {0}error.";
        public PlyFile? PlyFile { get; }

		public PlyModelException(PlyFile? plyFile = null, string? message = null) : base(GetFullErrorMessage(plyFile, message)) {
            PlyFile = plyFile;
        }
        public PlyModelException(string message, Exception inner, PlyFile? plyFile = null) : base(GetFullErrorMessage(plyFile, message), inner) {
            PlyFile = plyFile;
        }

        private static string GetFullErrorMessage(PlyFile? plyFile = null, string ? message = null) {
            var fullMessage = new StringBuilder();

            if (plyFile != null) {
                fullMessage.Append(string.Format(MESSAGE, $"\"{plyFile.Name}\" "));
            } else {
                fullMessage.Append(MESSAGE);
            }

            if (message != null) {
                fullMessage.Append(' ');
                fullMessage.Append(message);
            }

            return fullMessage.ToString();
        }


        public static PlyModelException NoContainMeshTextureName(PlyFile? plyFile, string meshTextureName) {
            return new PlyModelException(plyFile, $"Ply model don't contain mesh with \"{meshTextureName}\" texture name.");
        }

        public static PlyModelException AttemptInstallInvalidMtlTexture(PlyFile? plyFile, MtlTexture mtlTexture) {
            return new PlyModelException(plyFile, $"Attempt set invalid texture \"{mtlTexture.Diffuse.Name}\".");
        }
    }
}
