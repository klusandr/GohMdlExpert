using GohMdlExpert.Models.GatesOfHell.Exceptions;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceElement {

        public string Name { get; set; }
        public string? Path { get; set; }
        public string? RelativePathPoint { get; set; }
        public bool IsRelativePath => RelativePathPoint != null;

        public GohResourceElement(string name, string? path = null, string? relativePathPoint = null) {
            Name = name;
            Path = path;
            RelativePathPoint = relativePathPoint;
        }

        public string GetFullPath() {
            string path = Path ?? "";

            if (!SystemPath.IsPathFullyQualified(path) && RelativePathPoint != null) {
                path = SystemPath.Combine(RelativePathPoint, path);
            }

            return SystemPath.Combine(path, Name);
        }

        public string GetDirectoryPath() {
            return SystemPath.GetDirectoryName(GetFullPath())!; 
        }
    }
}