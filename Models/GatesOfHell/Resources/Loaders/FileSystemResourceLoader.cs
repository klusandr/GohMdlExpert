using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class FileSystemResourceLoader : GohBaseResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        protected string? _resourcePath;

        public override GohResourceDirectory? Root { get; protected set; }

        public override bool CheckRootPath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceNeedDirectories.All((d) => directories.Contains(d));
        }

        public override void LoadData(string path) {
            if (!CheckRootPath(path)) {
                throw GohResourceLoadException.IsNotGohResource(path);
            }

            _resourcePath = path;

            Root = new GohResourceDirectory("") { Loader = new FileSystemDirectoryLoader(this) };
        }

        public string GetFileSystemPath(string insidePath) {
            if (_resourcePath == null) {
                throw GohResourceLoadException.IsNotLoad();
            }

            return Path.Join(_resourcePath, insidePath);
        }

        public string GetResourceInsidePath(string fileSystemPath) {
            if (_resourcePath == null) {
                throw GohResourcesException.IsNotLoad();
            }

            return fileSystemPath.Replace(_resourcePath + '\\', null);
        }
    }
}
