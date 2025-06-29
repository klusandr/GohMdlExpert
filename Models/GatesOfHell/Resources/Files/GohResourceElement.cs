using Windows.Networking.Vpn;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceElement {
        private string? _path;
        private string? _relativePathPoint;
        private string _name;

        private string? _fullPath;
        private string? _fullDirectoryPath;

        public string Name {
            get => _name;
            set {
                _name = value;
                _fullPath = null;
            }
        }

        public string? Path {
            get => _path;
            set {
                _path = value;
                _fullDirectoryPath = null;
            }
        }

        public string? RelativePathPoint {
            get => _relativePathPoint;
            set {
                _relativePathPoint = value;
                _fullDirectoryPath = null;
            }
        }
        public bool IsRelativePath => RelativePathPoint != null;

        public GohResourceElement(string name, string? path = null, string? relativePathPoint = null) {
            _name = name;
            Path = path;
            RelativePathPoint = relativePathPoint;
        }

        public string GetFullPath() {
            return _fullPath ??= SystemPath.Join(GetDirectoryPath(), Name);
        }

        public string? GetDirectoryPath() {
            if (_fullDirectoryPath != null) {
                return _fullDirectoryPath;
            }

            string? path = Path;

            if (RelativePathPoint != null) {
                if (path != null) {
                    var basePathElements = new List<string>(RelativePathPoint.Split('\\', StringSplitOptions.RemoveEmptyEntries));
                    var pathElements = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var element in pathElements) {
                        if (element == "..") {
                            basePathElements.RemoveAt(basePathElements.Count - 1);
                        } else {
                            basePathElements.Add(element);
                        }
                    }

                    path = string.Join('\\', basePathElements);
                } else {
                    path = RelativePathPoint;
                }
            }

            return _fullDirectoryPath = path;
        }

        public override string ToString() {
            return GetFullPath();
        }
    }
}