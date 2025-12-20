using System.IO;
using System.IO.Compression;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;
using Windows.Networking.Sockets;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class PakResourceLoader : IGohResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture", "interface"
        };

        private static readonly List<string> s_resourcePakArchives = [
            (@"\entity\humanskin.pak"),
            (@"\texture\common\_hum.pak")
        ];

        public const string PATH_SEPARATOR = "/";

        public GohResourceDirectory? Root { get; protected set; }

        public PakResourceLoader(string path) {
            if (!CheckResourcePath(path)) {
                throw GohResourceLoadException.IsNotGohResource(path);
            }

            var rootDirectory = new GohResourceDirectory("") { Loader = new PakVirtualDirectoryLoader(this) };

            foreach (var archive in s_resourcePakArchives) {
                string fullPath = Path.Join(path, archive);
                var pathDirectories = PathUtils.GetPathComponents(PathUtils.GetPathWithoutExtension(archive));

                GohResourceDirectory currentDirectory = rootDirectory;

                foreach (var directoryName in pathDirectories) {
                    var directory = new GohResourceDirectory(directoryName, currentDirectory.GetFullPath()); 

                    if (directoryName != pathDirectories.Last()) {
                        directory.Loader = new PakVirtualDirectoryLoader(this);
                    } else {
                        directory.Loader = new PakDirectoryLoader(ZipFile.OpenRead(fullPath), this, directory.GetDirectoryPath()!) { PakPath = fullPath };
                    }

                    ((PakVirtualDirectoryLoader)currentDirectory.Loader).AddPakDirectory(directory);
                    currentDirectory = directory;
                }
            }

            Root = rootDirectory;
        }

        public virtual bool CheckResourcePath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);

            return s_resourceNeedDirectories.All((d) => directories.Contains(d))
                && s_resourcePakArchives.All(p => File.Exists(Path.Join(path, p)));
        }
    }
}
