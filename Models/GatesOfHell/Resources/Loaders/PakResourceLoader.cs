using System.IO;
using System.IO.Compression;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class PakResourceLoader : GohResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture", "interface"
        };

        private static readonly List<(string Path, string InsidePath)> s_resourcePakArchives = [
            (@"\entity\humanskin.pak", @"\entity\"),
            (@"\texture\common\_hum.pak", @"\texture\common\")
        ];


        public override GohResourceDirectory? Root { get; protected set; }

        public PakResourceLoader() {}

        public override bool CheckBasePath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);

            return s_resourceNeedDirectories.All((d) => directories.Contains(d)) 
                && s_resourcePakArchives.All(p => File.Exists(Path.Join(path, p.Path)) );
        }

        public override void LoadData(string path) {
            if (!CheckBasePath(path)) {
                throw GohResourcesException.IsNotGohResource(path);
            }

            var rootDirectoryLoader = new PakRootDirectoryLoader();
            var rootDirectory = new GohResourceDirectory(GohResourceLoading.ResourceDirectoryName) { Loader = rootDirectoryLoader };

            foreach (var archive in s_resourcePakArchives) {
                var pathDirectories = archive.InsidePath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                GohResourceDirectory? currentDirectory = null;

                foreach (var directoryName in pathDirectories) {
                    var directory = new GohResourceDirectory(directoryName) { Items = [] };

                    if (currentDirectory == null) {
                        rootDirectoryLoader.AddPakDirectory(directory);
                    } else {
                        currentDirectory.Items.Add(directory);
                    }

                    currentDirectory = directory;
                }

                string fullPath = Path.Join(path, archive.Path);

                var directoryLoad = new PakDirectoryLoader(ZipFile.OpenRead(fullPath)) { PakPath = fullPath };
                currentDirectory!.ClearData();
                currentDirectory!.Loader = directoryLoad;
            }

            Root = rootDirectory;
        }
    }
}
