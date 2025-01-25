using System.IO;
using System.Linq.Expressions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using Microsoft.Extensions.DependencyInjection;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files
{
    public class GohResourceFile : GohResourceElement {
        private object? _data;
        private IFileLoader? _fileLoader;

        public IFileLoader FileLoader { get => _fileLoader ?? GohServicesProvider.Instance.GetRequiredService<IFileLoader>(); init => _fileLoader = value; }

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
            return FileLoader.GetAllText(GetFullPath());
        }

        public Stream GetStream() {
            return FileLoader.GetStream(GetFullPath());
        }

#warning Слишком частый вызов при загрузке одной моделей или текстур к ней.
        public bool Exists() {
            return Path != null && FileLoader.Exists(GetFullPath()); ;
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

        public override bool Equals(object? obj) {
            if (obj == null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetHashCode() == GetHashCode()) {
                return true;
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
