using System.IO.Compression;
using GohMdlExpert.Models.GatesOfHell.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories {
    public class PakDirectoryLoader : IDirectoryLoader {
        private readonly ZipArchive _archive;
        private readonly PakFileLoader _fileLoader;
        private string? _pakPath;

        public PakDirectoryLoader(ZipArchive resourceArchive) {
            _archive = resourceArchive;
            _fileLoader = new PakFileLoader(resourceArchive);
        }

        public string? PakPath {
            get => _pakPath;
            set {
                _fileLoader.PakPath = value;
                _pakPath = value;
            }
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            string archivePath = _archive.GetArchiveDirectoryPath(path);
            var directories = new List<GohResourceDirectory>();
            int pathDeep = archivePath != "" ? ZipArchiveExtensions.GetDeep(archivePath) + 1 : 0;

            var directoriesPaths = _archive.GetDirectoriesPathsByDeep(pathDeep).Where(p => p.Contains(archivePath));

            foreach (var directoryPath in directoriesPaths) {
                directories.Add(new GohResourceDirectory(ZipArchiveExtensions.GetFullPath(path, archivePath, directoryPath)) { Loader = this });
            }

            return directories;
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            string archivePath = _archive.GetArchiveDirectoryPath(path);
            var files = new List<GohResourceFile>();
            int pathDeep = archivePath != "" ? ZipArchiveExtensions.GetDeep(archivePath) + 1 : 0;

            var filesEntries = _archive.Entries.Where(e => ZipArchiveExtensions.GetDeep(e.FullName) == pathDeep && !ZipArchiveExtensions.CheckDirectory(e.FullName) && e.FullName.Contains(archivePath));

            foreach (var entry in filesEntries) {
                files.Add(GohResourceLoading.CreateResourceFile(ZipArchiveExtensions.GetFullPath(path, archivePath, entry.FullName), fileLoader: _fileLoader));
            }

            return files;
        }
    }
}
