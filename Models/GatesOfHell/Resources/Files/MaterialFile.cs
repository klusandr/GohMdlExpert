using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public abstract class MaterialFile : GohResourceFile {
        protected MaterialFile(string name, string extension, string? path = null, string? relativePathPoint = null) : base(GetNameWithExtension(name, extension), path, relativePathPoint) { }

        public new DiffuseMaterial Data => GetMaterial();
        public ImageSource? ImageSource => (Data.Brush as ImageBrush)?.ImageSource;

        public override string? GetExtension() {
            return null;
        }

        public override void LoadData() { }

        protected static string GetNameWithExtension(string name, string extension) {
            return name.Contains(extension) ? name : name + extension;
        }

        protected virtual DiffuseMaterial GetMaterial() {
            return GohResourceLoading.LoadMaterial(this);
        }
    }
}
