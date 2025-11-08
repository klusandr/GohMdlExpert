using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class FileSystemResourceLoader : IGohResourceLoader {
        private static readonly string[] s_resourceDirectories = [
            "entity", "texture"
        ];

        protected string? _resourcePath;

        public FileSystemDirectoryLoader DirectoryLoader { get; }

        public FileSystemFileLoader FileLoader { get; }

        public GohResourceDirectory Root { get; }

        public FileSystemResourceLoader(string path) {
            if (!CheckResourceDirectory(path)) {
                throw GohResourceLoadException.IsNotGohResource(path);
            }

            _resourcePath = path;

            FileLoader = new FileSystemFileLoader(this);
            DirectoryLoader = new FileSystemDirectoryLoader(this, FileLoader);
            Root = new GohResourceDirectory("") { Loader = DirectoryLoader };
        }

        public virtual bool CheckResourceDirectory(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceDirectories.Any((d) => directories.Contains(d));
        }

        public string GetFileSystemPath(string insidePath) {
            if (_resourcePath == null) {
                throw GohResourceLoadException.IsNotLoad();
            }

            return Path.Join(_resourcePath, insidePath);
        }

        public string GetResourceInsidePath(string fileSystemPath) {
            if (_resourcePath == null) {
                throw GohResourceLoadException.IsNotLoad();
            }

            return fileSystemPath.Replace(_resourcePath + '\\', null);
        }
    }
}
