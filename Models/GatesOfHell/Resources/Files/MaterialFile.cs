using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class MaterialFile : GohResourceFile {
        public static string Extension => ".dds";
        public static string Extension2 => ".ebm";

        public MaterialFile(string name, string? path = null, string? relativePathPoint = null) : base(GetNameWithExtension(name), path, relativePathPoint) { }

        public new DiffuseMaterial Data => GetMaterial();
        public ImageSource? ImageSource => (Data.Brush as ImageBrush)?.ImageSource;

        public override string? GetExtension() {
            return null;
        }

        public override void LoadData() { }

        private static string GetNameWithExtension(string name) {
            return (name.Contains(Extension) || name.Contains(Extension2)) ? name : name + Extension;
        }

        protected virtual DiffuseMaterial GetMaterial() {
            return GohResourceLoading.LoadMaterial(this);
        }
    }
}
