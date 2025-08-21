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

            //if (!File.Exists(fullPath)) {
            //    throw GohResourceFileException.IsNotExists(fullPath);
            //}

            return new FileStream(fullPath, FileMode.OpenOrCreate);
        }

        public string GetAllText(string path) {
            using var stream = GetStream(path);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}