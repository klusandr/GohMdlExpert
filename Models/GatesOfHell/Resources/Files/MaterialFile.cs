using System.Windows.Media;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class MaterialFile : GohResourceFile {
        public static string Extension => ".dds";

        public MaterialFile(string name, string? path = null, string? relativePathPoint = null) : base(GetNameWithExtension(name), path, relativePathPoint) { }

        public new DiffuseMaterial Data => GetMaterial();
        public ImageSource? ImageSource => (Data.Brush as ImageBrush)?.ImageSource;

        public override string? GetExtension() {
            return Extension;
        }

        public override void LoadData() { }

        private static string GetNameWithExtension(string name) {
            return name.Contains(Extension) ? name : name + Extension;
        }

        private DiffuseMaterial GetMaterial() {
            return GohResourceLoading.LoadMaterial(this);
        }
    }
}
