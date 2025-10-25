using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourceSaveException : GohException {
        private const string MESSAGE = "GoH resource save error.";
        protected override byte ExceptionTypeCode => 9;

        public GohResourceSaveException(string? message = null, Exception? inner = null) : base(GetFullMessage(message), inner) { }

        public static GohResourceSaveException OutputModIsNotSet() {
            return new GohResourceSaveException($"Output mod is not in set.") { ExceptionCode = 1 };
        }

        public static GohResourceSaveException FileOutputPathIsNotModPath(string outputPath, GohOutputMod mod) {
            return new GohResourceSaveException(string.Format("File output path not include path of output mod. Output path: \"{0}\". Output mod path: \"{0}\".", mod.Path)) { ExceptionCode = 2 };
        }

        public static GohResourceSaveException SaveReadOnlyFile(GohResourceFile file) {
            return new GohResourceSaveException(string.Format("Try save read only file. File name: {0}", file.GetFullPath())) { ExceptionCode = 3 };
        }

        public static GohResourceSaveException MtlTextureIsNotDefindForMesh(string textureName) {
            return new GohResourceSaveException(string.Format("Mtl texture is not define for mesh \"{0}\".", textureName)) { ExceptionCode = 4 };
        }

        public static GohResourceSaveException SaveFilePathIsNotDefine(MdlFile mdlFile) {
            return new GohResourceSaveException(string.Format("File output path is not define. File name: \"{0}\".", mdlFile.Name)) { ExceptionCode = 5 };
        }

        public static GohResourceSaveException SaveReadOnlyFile(GohResourceFile file) {
            return new GohResourceSaveException(string.Format("Try save read only file. File name: {0}", file.GetFullPath()));
        }

        private static string GetFullMessage(string? message) {
            if (message == null) {
                return MESSAGE;
            } else {
                return MESSAGE + " " + message;
            }
        }

    }
}
