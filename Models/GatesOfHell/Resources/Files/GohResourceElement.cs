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
            return SystemPath.Join(GetDirectoryPath(), Name);
        }

        public string? GetDirectoryPath() {
            string? path;

            if (RelativePathPoint != null) {
                path = SystemPath.Join(RelativePathPoint, Path);
            } else {
                path = Path;
            }

            return path;
        }

        public override string ToString() {
            return GetFullPath();
        }
    }
}