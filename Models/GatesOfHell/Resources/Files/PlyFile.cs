using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Serialization;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files
{
    public class PlyFile : GohResourceFile {
        private static PlySerializer? s_serializer;

        private static PlySerializer Serializer => s_serializer ??= new PlySerializer();

        public new PlyModel Data { get => (PlyModel)base.Data; set => base.Data = value; }

        public static string Extension => ".ply";

        public PlyFile(string name, string? path = null, string? relativePathPoint = null)
            : base(name, path, relativePathPoint) { }

        public override string? GetExtension() {
            return Extension;
        }

        public override void LoadData() {
            using var stream = GetStream();

            Data = Serializer.Deserialize(stream);
        }
    }
}
