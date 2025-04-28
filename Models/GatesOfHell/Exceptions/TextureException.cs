using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class TextureException : GohException {
        private const string MESSAGE = "Gates of hell {0}texture {1}error.{2}";
        private const string MESSAGE_MATERIAL = "material {0} from ";

        protected override byte ExceptionTypeCode => 4;

        public MtlFile? MtlFile { get; }
        public MaterialFile? MaterialFile { get; }

        public TextureException(string? message = null, MtlFile? mtlFile = null, MaterialFile? materialFile = null, Exception? inner = null)
            : base(GetFullMessage(message, mtlFile, materialFile), inner) {
            MtlFile = mtlFile;
            MaterialFile = materialFile;
        }

        public static TextureException DirectoryNotSpecified() {
            return new TextureException("Texture directory is not specified.") { ExceptionCode = 1 };
        }

        public static TextureException NotBelongPlyModel(AggregateMtlFile mtlFile, PlyFile? plyFile = null) {
            return new TextureException(string.Format("Aggregate texture \"{0}\" don't belong to the ply model{1}.",
                mtlFile.Name,
                plyFile != null ? " \"" + plyFile.GetFullPath() + '"' : string.Empty)) { ExceptionCode = 2 };
        }

        public static TextureException NotBelongPlyModel(AggregateMtlFiles mtlFiles) {
            return new TextureException(string.Format("Aggregate textures don't belong to the ply model \"{0}\".", mtlFiles.PlyFile.GetFullPath())) { ExceptionCode = 3 };
        }

        public static TextureException AggregateFilesInconsistency(AggregateMtlFile aggregateFile1, AggregateMtlFile aggregateFile2) {
            return new TextureException(string.Format("Aggregate texture for ply model \"{0}\" inconsistency with aggregate texture for \"{1}\". Texture name \"{2}\".",
                aggregateFile1.PlyFile.GetFullPath(),
                aggregateFile2.PlyFile.GetFullPath(),
                aggregateFile1.Name)) { ExceptionCode = 3 };
        }

        public static TextureException TextureDiffuseMaterialIsNotDefine(MtlFile? mtlFile = null) {
            return new TextureException("Diffuse material parameter not define.", mtlFile) { ExceptionCode = 4 };
        }

        public static TextureException TextureAlreadyContained(MtlTexture mtlTexture) {
            return new TextureException("The texture is already contained in the texture collection.", materialFile: mtlTexture.Diffuse) { ExceptionCode = 5 };
        }

        public static TextureException MaterialsNotInitialize(MtlTexture mtlTexture) {
            return new TextureException("The texture materials is not initialize.") { ExceptionCode = 6 };
        }

        private static string GetFullMessage(string? message = null, MtlFile? mtlFile = null, MaterialFile? materialFile = null) {
            return string.Format(MESSAGE,
                materialFile != null ? string.Format(MESSAGE_MATERIAL, "\"" + materialFile.GetFullPath() + "\" ") : string.Empty,
                mtlFile != null ? "\"" + mtlFile.GetFullPath() + "\" " : string.Empty,
                message != null ? ' ' + message : string.Empty);
        }
    }
}
