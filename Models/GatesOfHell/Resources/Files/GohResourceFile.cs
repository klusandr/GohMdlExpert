using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceFile : GohResourceElement {
        private object? _data;

        public object Data {
            get {
                if (_data == null) {
                    LoadData();
                }

                return _data!;
            }
            set => _data = value;
        }

        public GohResourceFile(string name, string? path = null, string? relativePathPoint = null) : base(name, path, relativePathPoint) {
            if (GetExtension() != null && SystemPath.GetExtension(name) != GetExtension()) {
                throw GohResourceFileException.InvalidExtension(this, GetExtension()!);
            }

            if (path == null) {
                Path = SystemPath.GetDirectoryName(name);

                if (Path == "") { Path = null; }

                Name = SystemPath.GetFileName(name);
            }
        }

        public string GetAllText() {
            string path = GetFullPath();

            if (!Exists()) {
                throw GohResourceFileException.IsNotExists(this);
            }

            return File.ReadAllText(path);
        }

        public FileStream GetStream() {
            string path = GetFullPath();

            if (!Exists()) {
                throw GohResourceFileException.IsNotExists(this);
            }

            return new FileStream(path, FileMode.Open);
        }

        public virtual string? GetExtension() {
            return null;
        }

        public virtual void LoadData() {
            Data = GetAllText();
        }

        public virtual void UnloadData() {
            _data = null;
        }

        public virtual void SaveData() {
            throw new NotImplementedException();
        }
#warning Слишком частый вызов при загрузке одной моделей или текстур к ней.
        public bool Exists() {
            return Path != null && File.Exists(GetFullPath());
        }

        public override bool Equals(object? obj) {
            if (obj == null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj is GohResourceFile file) {
                return GetFullPath() == file.GetFullPath();
            }

            return false;
        }

        public override int GetHashCode() {
            return Name.GetHashCode()
                + Path?.GetHashCode() ?? 0
                + RelativePathPoint?.GetHashCode() ?? 0;
        }
    }
}
