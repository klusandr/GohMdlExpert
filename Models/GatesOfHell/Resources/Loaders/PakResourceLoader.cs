using System.IO;
using System.IO.Compression;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class PakResourceLoader : IGohResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture", "interface"
        };

        private static readonly List<(string Path, string InsidePath)> s_resourcePakArchives = [
            (@"\entity\humanskin.pak", @"\entity\humanskin"),
            (@"\texture\common\_hum.pak", @"\texture\common\_hun")
        ];

        public GohResourceDirectory? Root { get; protected set; }

        public PakResourceLoader(string path) {
            if (!CheckResourcePath(path)) {
                throw GohResourceLoadException.IsNotGohResource(path);
            }

            var rootDirectoryLoader = new PakRootDirectoryLoader(this);
            var rootDirectory = new GohResourceDirectory("") { Loader = rootDirectoryLoader };

            foreach (var archive in s_resourcePakArchives) {
                var pathDirectories = archive.InsidePath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                GohResourceDirectory? currentDirectory = null;
                string fullPath = Path.Join(path, archive.Path);
                var directoryLoaders = new PakDirectoryLoader(ZipFile.OpenRead(fullPath), this) { PakPath = fullPath };

                foreach (var directoryName in pathDirectories) {
                    var directory = new GohResourceDirectory(directoryName, currentDirectory?.GetFullPath()) { Loader = directoryLoaders };

                    rootDirectoryLoader.AddPakDirectory(directory);

                    currentDirectory = directory;
                }

                currentDirectory!.ClearData();
            }

            Root = rootDirectory;
        }

        public virtual bool CheckResourcePath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);

            return s_resourceNeedDirectories.All((d) => directories.Contains(d))
                && s_resourcePakArchives.All(p => File.Exists(Path.Join(path, p.Path)));
        }
    }
}
