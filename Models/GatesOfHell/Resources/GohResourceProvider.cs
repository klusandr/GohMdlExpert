using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohResourceProvider {
        private static readonly Dictionary<string, Type> s_fileTypes = new() {
            [".mdl"] = typeof(MdlFile),
            [".ply"] = typeof(PlyFile),
            [".mtl"] = typeof(MtlFile),
            [".dds"] = typeof(MaterialFile),
        };

        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        public GohResourceLocations ResourceLocations { get; }

        public GohResourceDirectory? ResourceDictionary { get; private set; }
        public bool IsResourceLoaded => ResourceDictionary != null;

        public event EventHandler? ResourceUpdated;

        public GohResourceProvider(GohResourceLocations resourceLocations) {
            ResourceLocations = resourceLocations;
        }

        public void OpenResources(string path) {
            if (!CheckGohResourceDirectory(path)) {
                throw GohResourcesException.IsNotGohResource(path);
            }

            ResourceDictionary = new GohResourceDirectory(path);
            OnResourceUpdated();
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            if (ResourceDictionary == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            string locationPath = ResourceLocations.GetLocationPath(location);
            var findDirectory = ResourceDictionary.AlongPath(locationPath);

            return findDirectory ?? throw GohResourcesException.LocationNotFound(location, locationPath);
        }

        public GohResourceDirectory GetResourceDirectory(GohResourceElement resourceElement) {
            if (ResourceDictionary == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            var path = resourceElement.GetDirectoryPath() ?? throw GohResourcesException.PathIsNull(resourceElement);

            if (ResourceDictionary.GetFullPath().Contains(path)) {
                throw GohResourcesException.ElementNotInResource(resourceElement);
            }

            return ResourceDictionary.AlongPath(path.Replace(ResourceDictionary.GetFullPath(), string.Empty)) ?? throw GohResourcesException.PathIsNull(resourceElement);
        }

        public static GohResourceFile GetResourceFile(string fileName, string? path = null) {
            if (s_fileTypes.TryGetValue(Path.GetExtension(fileName), out var fileType)) {
                return (GohResourceFile)fileType.GetConstructors()[0].Invoke([fileName, path, null])!;
            } else {
                return new GohResourceFile(fileName, path);
            }
        }

        private static bool CheckGohResourceDirectory(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceNeedDirectories
                .All((d) => directories
                    .Contains(d)
                );
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
