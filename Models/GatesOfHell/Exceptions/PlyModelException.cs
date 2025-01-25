using System.Text;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions
{
    public class PlyModelException : GohException {
        private const string MESSAGE = "Gates of hell ply model {0}error.";
        public PlyFile? PlyFile { get; }

        public PlyModelException(string? message = null, PlyFile? plyFile = null, Exception? inner = null) : base(GetFullErrorMessage(plyFile, message), inner) {
            PlyFile = plyFile;
        }

        private static string GetFullErrorMessage(PlyFile? plyFile = null, string? message = null) {
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
            return new PlyModelException($"Ply model don't contain mesh with \"{meshTextureName}\" texture name.", plyFile);
        }

        public static PlyModelException AttemptInstallInvalidMtlTexture(PlyFile? plyFile, MtlTexture mtlTexture) {
            return new PlyModelException($"Attempt set invalid texture \"{mtlTexture.Diffuse.Name}\".", plyFile);
        }
    }
}
