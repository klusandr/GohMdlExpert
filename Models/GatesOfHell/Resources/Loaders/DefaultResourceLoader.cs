using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class DefaultResourceLoader : IGohResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        private static readonly Dictionary<string, string> s_locationsPaths = new() {
            ["texture"] = @"\texture\common",
            ["ger_humanskin"] = @"\entity\humanskin\[germans]",
            ["us_humanskin"] = @"\entity\humanskin\[united_states]",
            ["sov_humanskin"] = @"\entity\humanskin\[soviets]",
        };

        public GohResourceDirectory? Root { get; private set; }

        public bool CheckBasePath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceNeedDirectories.All((d) => directories.Contains(d));
        }

        public void LoadData(string path) {
            if (!CheckBasePath(path)) {
                throw GohResourcesException.IsNotGohResource(path);
            }

            Root = new GohResourceDirectory(path) { Loader = new DefaultDirectoryLoader() };
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            if (!s_locationsPaths.TryGetValue(location, out string? path)) {
                throw GohResourcesException.LocationNotDefined(location);
            }

            return GetDirectory(path) ?? throw GohResourcesException.LocationNotFound(location, path);
        }

        public GohResourceDirectory? GetDirectory(string path) {
            if (Root == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            if (Path.IsPathFullyQualified(path)) {
                if (path.Contains(Root.GetFullPath())) {
                    path = path.Replace(Root.GetFullPath(), null);
                } else {
                    throw GohResourcesException.ElementNotInResource(path);
                }
            }

            return Root.AlongPath(path);
        }

        public GohResourceFile? GetFile(string fullName) {
            if (Root == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            string? path = Path.GetDirectoryName(fullName);
            GohResourceDirectory? directory;

            if (path != null) {
                try {
                    directory = GetDirectory(path);
                } catch (GohResourcesException) {
                    throw GohResourcesException.ElementNotInResource(fullName);
                }
            } else {
                directory = Root;
            }

            string name = Path.GetFileName(fullName);

            return directory?.GetFile(name);
        }
    }
}
