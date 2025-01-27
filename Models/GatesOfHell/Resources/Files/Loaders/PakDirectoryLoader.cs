using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class PakDirectoryLoader : IDirectoryLoader {
        private ZipArchive _archive;
        private readonly IFileLoader _fileLoader;

        public PakDirectoryLoader(IFileLoader fileLoader) {
            _archive = ZipFile.OpenRead(@"F:\Steam Game\steamapps\common\Call to Arms - Gates of Hell\resource\entity\humanskin.pak");
            _fileLoader = fileLoader;
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            path = path.Replace("\\", "/");
            var directories = new List<GohResourceDirectory>();

            var directoriesEntries = _archive.Entries
                .Where(e => e.FullName.Contains(path) 
                    && CheckDirectory(e.FullName) 
                    && CheckDeep(path, e.FullName) == 1);

            foreach (var entry in directoriesEntries) {
                directories.Add(new GohResourceDirectory(entry.FullName.Trim('/')) { Loader = this });
            }

            return directories;
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            path = path.Replace("\\", "/");
            var files = new List<GohResourceFile>();

            var directoriesEntries = _archive.Entries
                .Where(e => e.FullName.Contains(path)
                    && !CheckDirectory(e.FullName)
                    && CheckDeep(path, e.FullName) == 0);

            foreach (var entry in directoriesEntries) {
                files.Add(GohResourceLoading.GetResourceFile(entry.FullName, fileLoader: _fileLoader));
            }

            return files;
        }

        private static bool CheckDirectory(string entryName) {
            return entryName[^1] == '/';
        }

        private static int CheckDeep(string path, string entryName) {
            return entryName.Replace(path, string.Empty).Count((c) => c == '/') - 1;
        }
    }
}
