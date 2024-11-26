using GohMdlExpert.Models.GatesOfHell.Exceptions;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceElement {

        public string Name { get; set; }
        public string? Path { get; set; }
        public bool RelativePath => RelativePathPoint != null || ResourceLocation != null;
        public string? RelativePathPoint { get; set; }
        public string? ResourceLocation { get; set; }

        public GohResourceElement(string name, string? path = null, string? relativePathPoint = null, string? location = null) {
            Name = name;
            Path = path;
            ResourceLocation = location;
            RelativePathPoint = relativePathPoint;
        }

        public string GetFullPath() {
            string path = Path ?? "";

            if (!SystemPath.IsPathFullyQualified(path) && RelativePath) {
                string absolutePath;

                if (ResourceLocation != null) {
                    absolutePath = SystemPath.Join(GohResourceLocations.Instance.GetLocationFullPath(ResourceLocation), path);
                } else {
                    absolutePath = SystemPath.GetFullPath(SystemPath.Combine(RelativePathPoint!, path));
                }

                path = absolutePath;
            }

            return SystemPath.Combine(path, Name);
        }

        public string GetDirectoryPath() {
            return SystemPath.GetDirectoryName(GetFullPath())!; 
        }
    }
}