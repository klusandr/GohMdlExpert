using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class FileSystemResourceLoader : GohBaseResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        protected string? _resourcePath;
        private FileSystemDirectoryLoader? _directoryLoader;
        private FileSystemFileLoader? _fileLoader;
        private GohResourceDirectory? _root;

        public FileSystemDirectoryLoader DirectoryLoader { 
            get => _directoryLoader ?? throw GohResourceLoadException.LoaderNotLoadResource(this);
            private set => _directoryLoader = value; 
        }

        public FileSystemFileLoader FileLoader { 
            get => _fileLoader ?? throw GohResourceLoadException.LoaderNotLoadResource(this);  
            private set => _fileLoader = value; 
        }
        public override GohResourceDirectory Root {
            get => _root ?? throw GohResourceLoadException.LoaderNotLoadResource(this);
            protected set => _root = value; 
        }

        public FileSystemResourceLoader(string path) {
            if (!CheckRootPath(path)) {
                throw GohResourceLoadException.IsNotGohResource(path);
            }

            _resourcePath = path;
            FileLoader = new FileSystemFileLoader(this);
            DirectoryLoader = new FileSystemDirectoryLoader(this, FileLoader);

        public override void LoadData() {
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
                throw GohResourceLoadException.IsNotLoad();
            }

            return fileSystemPath.Replace(_resourcePath + '\\', null);
        }
    }
}
