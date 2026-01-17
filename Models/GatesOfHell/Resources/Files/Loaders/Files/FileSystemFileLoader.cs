using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files {
    public class FileSystemFileLoader : IFileLoader {
        private readonly FileSystemResourceLoader _resourceLoader;

        public virtual bool IsReadOnly => false;

        public IGohResourceLoader ResourceLoader => _resourceLoader;

        public FileSystemFileLoader(FileSystemResourceLoader resourceLoader) {
            _resourceLoader = resourceLoader;
        }

        public bool Exists(string path) {
            return File.Exists(_resourceLoader.GetFileSystemPath(path));
        }

        public Stream GetStream(string path) {
            string fullPath = _resourceLoader.GetFileSystemPath(path);

            return new FileStream(fullPath, FileMode.OpenOrCreate);
        }

        public string GetAllText(string path) {
            string fullPath = _resourceLoader.GetFileSystemPath(path);

            if (!File.Exists(fullPath)) {
                throw GohResourceFileException.IsNotExists(fullPath);
            }

            return File.ReadAllText(fullPath);
        }

        public void WriteAllText(string path, string text) {
            string fullPath = _resourceLoader.GetFileSystemPath(path);
            File.WriteAllText(fullPath, text);
        }
    }
}