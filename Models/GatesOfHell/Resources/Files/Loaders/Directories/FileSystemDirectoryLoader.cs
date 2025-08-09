using System.IO;
using GohMdlExpert.Models.GatesOfHell.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories {
    public class FileSystemDirectoryLoader : IDirectoryLoader {
        protected readonly FileSystemResourceLoader _resourceLoader;
        private readonly FileSystemFileLoader _fileLoader;

        public IGohResourceLoader ResourceLoader => _resourceLoader;

        public FileSystemDirectoryLoader(FileSystemResourceLoader resourceLoader, FileSystemFileLoader fileLoader) {
            _resourceLoader = resourceLoader;
            _fileLoader = fileLoader;
        }

        public FileSystemDirectoryLoader(FileSystemResourceLoader resourceLoader) : this(resourceLoader, new FileSystemFileLoader(resourceLoader)) { }

        public virtual IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            var directories = new List<GohResourceDirectory>();
            string fullPath = _resourceLoader.GetFileSystemPath(path);


            foreach (var directoryNames in Directory.GetDirectories(fullPath)) {
                directories.Add(new GohResourceDirectory(_resourceLoader.GetResourceInsidePath(directoryNames)) { Loader = this });
            }

            return directories.OrderByNature(d => d.Name);
        }

        public virtual IEnumerable<GohResourceFile> GetFiles(string path) {
            var files = new List<GohResourceFile>();
            string fullPath = _resourceLoader.GetFileSystemPath(path);

            foreach (var fileFullPath in Directory.GetFiles(fullPath)) {
                files.Add(GohResourceLoading.CreateResourceFile(_resourceLoader.GetResourceInsidePath(fileFullPath), fileLoader: _fileLoader));
            }

            return files.OrderByNature(f => f.Name);
        }
    }
}
