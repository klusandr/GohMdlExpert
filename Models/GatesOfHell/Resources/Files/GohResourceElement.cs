using GohMdlExpert.Models.GatesOfHell.Exceptions;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceElement {

        public string Name { get; protected set; }
        public string? Path { get; protected set; }
        public string? RelativePathPoint { get; set; }
        public bool IsRelativePath => !SystemPath.IsPathFullyQualified(Path ?? string.Empty);

        public GohResourceElement(string name, string? path = null, string? relativePathPoint = null) {
            Name = name;
            Path = path;
            RelativePathPoint = relativePathPoint;
        }

        public string GetFullPath() {
            string path = Path ?? "";

            if (IsRelativePath && RelativePathPoint != null) {
                path = SystemPath.Join(RelativePathPoint, path);
            }

            return SystemPath.Join(path, Name);
        }

        public string? GetDirectoryPath() {
            string? path = SystemPath.GetDirectoryName(GetFullPath());

            return path != string.Empty ? path : null; 
        }
    }
}