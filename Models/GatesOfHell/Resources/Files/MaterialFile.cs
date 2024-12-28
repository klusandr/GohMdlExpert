using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class MaterialFile : GohResourceFile {
        public MaterialFile(string name, string? path = null, string? relativePathPoint = null) : base(name, path, relativePathPoint) {
            Name += ".dds";
        }

        public new Material Data => GetMaterial();

        public override void LoadData() { }

        private Material GetMaterial() {
            if (IsRelativePath && RelativePathPoint == null) {
                throw new GohResourceFileException("The path to the material file was incomplete.", GetFullPath());
            }

            return ResourceLoading.LoadMaterial(this);
        }
    }
}
