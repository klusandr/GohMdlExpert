using System.IO.Compression;
using GohMdlExpert.Models.GatesOfHell.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories {
    public class PakDirectoryLoader : IDirectoryLoader {
        private readonly ZipArchive _archive;
        private readonly PakResourceLoader _resourceLoader;
        private readonly PakFileLoader _fileLoader;
        private string? _pakPath;

        public IGohResourceLoader ResourceLoader => _resourceLoader;

        public string? PakPath {
            get => _pakPath;
            init {
                _fileLoader.PakPath = value;
                _pakPath = value;
            }
        }

        public string PakInsidePath { get; }

        public PakDirectoryLoader(ZipArchive resourceArchive, PakResourceLoader resourceLoader, string pakInsidePath) {
            _archive = resourceArchive;
            _resourceLoader = resourceLoader;
            _fileLoader = new PakFileLoader(resourceArchive, resourceLoader);
            PakInsidePath = pakInsidePath;
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            string archivePath = GetInsidePath(path);
            var directories = new List<GohResourceDirectory>();
            int pathDeep = ZipArchiveExtensions.GetDeep(archivePath) + 1;

            var directoriesPaths = new List<string>();

            foreach (var entrie in _archive.Entries) {
                if (entrie.FullName.Contains(archivePath)
                    && ZipArchiveExtensions.GetDeep(entrie.FullName) > pathDeep
                    && directoriesPaths.All(d => !entrie.FullName.Contains(d))) {
                    directoriesPaths.Add(ZipArchiveExtensions.GetPathFromFirstElments(entrie.FullName, pathDeep + 1) + '/');
                }
            }

            foreach (var directoryPath in directoriesPaths) {
                directories.Add(new GohResourceDirectory(GetOutsidePath(directoryPath)) { Loader = this });
            }

            return directories;
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            string archivePath = GetInsidePath(path) + '/';
            var files = new List<GohResourceFile>();
            int pathDeep = ZipArchiveExtensions.GetDeep(archivePath) + 1;

            var filesPaths = _archive.Entries
                .Where(e => e.FullName.Contains(archivePath)
                && ZipArchiveExtensions.GetDeep(e.FullName) == pathDeep
                && !ZipArchiveExtensions.CheckDirectory(e.FullName)
            ).Select(e => e.FullName);

            foreach (var filePath in filesPaths) {
                files.Add(GohResourceLoading.CreateResourceFile(GetOutsidePath(filePath), fileLoader: _fileLoader));
            }

            return files;
        }

        private string GetInsidePath(string path) {
            return ZipArchiveExtensions.GetZipPath(path.Replace(PakInsidePath, null).Trim(GohResourceLoading.DIRECTORY_SEPARATE));
        }

        private string GetOutsidePath(string path) {
            return PathUtils.GetPathFromComponents([PakInsidePath, path]);
        }
    }
}
