using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class DefaultDirectoryLoader : IDirectoryLoader {
        private readonly DefaultFileLoader _fileLoader;

        public DefaultDirectoryLoader() {
            _fileLoader = new DefaultFileLoader();
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            var directories = new List<GohResourceDirectory>();

            foreach (var directoryNames in Directory.GetDirectories(path)) {
                directories.Add(new GohResourceDirectory(directoryNames) { Loader = this });
            }

            return directories;
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            var files = new List<GohResourceFile>();

            foreach (var fileFullPath in Directory.GetFiles(path)) {
                files.Add(GohResourceLoading.GetResourceFile(fileFullPath, fileLoader: _fileLoader));
            }

            return files;
        }
    }
}
