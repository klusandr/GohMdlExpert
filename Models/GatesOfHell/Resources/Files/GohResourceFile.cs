using GohMdlExpert.Models.GatesOfHell.Exceptions;
using System.IO;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceFile : GohResourceElement {
        private object? _data;

        public virtual string? Extension => null;
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
            if (Extension != null && SystemPath.GetExtension(name) != Extension) {
                throw GohResourceFileException.InvalidExtension(this, Extension);
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

        public virtual void LoadData() {
            Data = GetAllText();
        }

        public virtual void UnloadData() {
            _data = null;
        }

        public virtual void SaveData() {
            throw new NotImplementedException();
        }

        public bool Exists() {
            return File.Exists(GetFullPath());
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
            return base.GetHashCode();
        }
    }
}
