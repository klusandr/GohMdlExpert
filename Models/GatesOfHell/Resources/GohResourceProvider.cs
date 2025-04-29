using System.IO;
using System.Reflection;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class GohResourceProvider {
        private static readonly Dictionary<string, Type> s_fileTypes = new() {
            [MtlFile.Extension] = typeof(MdlFile),
            [PlyFile.Extension] = typeof(PlyFile),
            [MtlFile.Extension] = typeof(MtlFile),
            [MaterialFile.Extension] = typeof(MaterialFile),
        };

        private static readonly IEnumerable<IGohResourceLoader> s_baseResourceDirectories = [
            new PakResourceLoader(),
            new ModResourceLoader()
        ];
        private IGohResourceLoader? _baseResourceLoader;
        private IGohResourceLoader? _currentResourceLoader;

        public IGohResourceLoader ResourceLoader => _currentResourceLoader ?? throw GohResourcesException.DirectoryNotSpecified();
        public GohResourceDirectory ResourceDirectory => ResourceLoader?.Root ?? throw GohResourcesException.DirectoryNotSpecified();
        public bool IsResourceLoaded => ResourceDirectory != null;

        public GohModResourceProvider ModResourceProvider { get; }

        public event EventHandler? ResourceUpdated;

        public GohResourceProvider(GohModResourceProvider modResourceProvider) {
            ModResourceProvider = modResourceProvider;
        }

        public void OpenResources(string path) {
            _baseResourceLoader = _currentResourceLoader = GetResourceLoader(path) ?? throw GohResourcesException.IsNotGohResource(path);
            _currentResourceLoader.LoadData(path);
            OnResourceUpdated();
        }

        public void LoadModResources() {
            if (_baseResourceLoader == null) {
                return;
            }

            var resourceLoaders = ModResourceProvider.Mods.Where(m => m.IsEnable && m.IsLoad).Select(m => m.ResourceLoader);

            if (resourceLoaders.Any()) {
                _currentResourceLoader = new AggregateResourceLoader([_baseResourceLoader, .. resourceLoaders]);
            } else {
                _currentResourceLoader = _baseResourceLoader;
            }

            OnResourceUpdated();
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            return ResourceLoader.GetLocationDirectory(location);
        }

        public GohResourceDirectory GetResourceDirectory(GohResourceElement resourceElement) {
            string? path = resourceElement.GetDirectoryPath();
            GohResourceDirectory? resourceDirectory;

            if (path != null) {
                resourceDirectory = ResourceLoader.GetDirectory(path);
            } else {
                resourceDirectory = ResourceDirectory;
            }

            if (resourceDirectory == null || resourceDirectory.GetFile(resourceElement.Name) == null) {
                throw GohResourcesException.ElementNotInResource(resourceElement);
            }

            return resourceDirectory;
        }

        public GohResourceDirectory? GetDirectory(string path) {
            if (ResourceDirectory == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            if (Path.IsPathFullyQualified(path)) {
                if (path.Contains(ResourceDirectory.GetFullPath())) {
                    path = path.Replace(ResourceDirectory.GetFullPath(), null);
                } else {
                    throw GohResourcesException.ElementNotInResource(path);
                }
            }

            return ResourceDirectory.AlongPath(path);
        }

        public GohResourceFile? GetFile(string fullName) {
            if (ResourceDirectory == null) {
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
                directory = ResourceDirectory;
            }

            string name = Path.GetFileName(fullName);

            return directory?.GetFile(name);
        }

        public string GetInsidePath(string path) {
            string insidePath = path;

            if (Path.IsPathFullyQualified(path)) {
                if (path.Contains(ResourceDirectory.GetFullPath())) {
                    insidePath = path.Replace(ResourceDirectory.GetFullPath(), null);
                }
            }

            return insidePath;
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }

        private IGohResourceLoader? GetResourceLoader(string path) {
            foreach (var resourceDirectory in s_baseResourceDirectories) {
                if (resourceDirectory.CheckBasePath(path)) {
                    return resourceDirectory;
                }
            }

            return null;
        }
    }
}
