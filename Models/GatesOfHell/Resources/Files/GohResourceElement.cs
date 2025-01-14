using GohMdlExpert.Models.GatesOfHell.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.DependencyInjection;
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
            return SystemPath.Join(GetDirectoryPath(), Name);
        }

        public string? GetDirectoryPath() {
            string? path;

            if (IsRelativePath) {
                path = SystemPath.Join(RelativePathPoint, Path);
            } else {
                path = Path;
            }

            return path; 
        }
    }
}