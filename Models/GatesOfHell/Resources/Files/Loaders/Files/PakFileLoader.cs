using System.IO;
using System.IO.Compression;
using System.Text;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files {
    public class PakFileLoader : IFileLoader {
        private readonly ZipArchive _archive;
        private readonly PakResourceLoader _resourceLoader;
        private readonly PakDirectoryLoader _pakDirectoryLoader;

        public bool IsReadOnly => true;

        public string? PakPath { get; set; }

        public IGohResourceLoader ResourceLoader => _resourceLoader;

        public PakFileLoader(ZipArchive archive, PakResourceLoader resourceLoader, PakDirectoryLoader pakDirectoryLoader) {
            _archive = archive;
            _resourceLoader = resourceLoader;
            _pakDirectoryLoader = pakDirectoryLoader;
        }

        public bool Exists(string path) {
            return GetEntry(path) != null;
        }

        public string GetAllText(string path) {
            using var stream = GetStream(path);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer);

            return Encoding.UTF8.GetString(buffer);
        }

        public Stream GetStream(string path) {
            var entry = GetEntry(path) ?? throw GohResourceFileException.IsNotExists(path);

            return entry.Open();
        }


        private ZipArchiveEntry? GetEntry(string path) {
            string insidePath = _pakDirectoryLoader.GetInsidePath(path);

            return _archive.Entries.FirstOrDefault(e => e.FullName.Equals(insidePath, StringComparison.CurrentCultureIgnoreCase));
            //return _archive.GetEntry(_archive.GetArchiveFilePath(path)) != null;
        }

        public void WriteAllText(string path, string text) {
            throw new NotImplementedException();
        }
    }
}
