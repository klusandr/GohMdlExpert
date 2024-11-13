using GohMdlExpert.Models.GatesOfHell.Exceptions;
using System.IO;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceFile {
        private object? _data;

        public string Name { get; set; }
        public string? Path { get; set; }
        public bool RelativePath => RelativePathPoint != null || ResourceLocation != null;
        public string? RelativePathPoint { get; set; }
        public string? ResourceLocation { get; set; }
        public virtual string? Extension => null;
        public object? Data {
            get {
                if (_data == null) {
                    LoadData();
                } 

                return _data;
            }
            set => _data = value;
        }

        public GohResourceFile(string name, string? path = null, string? relativePathPoint = null, string? location = null) {
            if (Extension != null && SystemPath.GetExtension(name) != Extension) {
                throw new GohResourceFileException($"File \"{GetFullPath}\" is not {Extension}.");
            }

            if (path == null) {
                path = SystemPath.GetDirectoryName(name);
                name = SystemPath.GetFileName(name);
            }
            
            Name = name;
            Path = path;
            ResourceLocation = location;
            RelativePathPoint = relativePathPoint;
        }

        public string GetAllText() {
            string path = GetFullPath();

            if (!File.Exists(path)) {
                throw new GohResourcesException("Resource file is not exists");
            }

            return File.ReadAllText(path);
        }

        public FileStream GetStream() {
            string path = GetFullPath();

            if (!File.Exists(path)) {
                throw new GohResourcesException("Resource file is not exists");
            }

            return new FileStream(path, FileMode.Open);
        }

        public string GetFullPath() {
            string path = Path ?? "";

            if (!SystemPath.IsPathFullyQualified(path) && RelativePath) {
                string absolutePath;

                if (ResourceLocation != null) {
                    absolutePath = SystemPath.Join(ResourceLocations.Instance.GetLocationPath(ResourceLocation), path);
                } else {
                    absolutePath = SystemPath.GetFullPath(SystemPath.Combine(RelativePathPoint!, path));
                }

                path = absolutePath;
            }

            return SystemPath.Combine(path, Name);
        }

        public virtual void LoadData() {
            Data = GetAllText();
        }
    }
}
