using System.IO;
using System.Reflection;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class GohResourceProvider {
        private static readonly IEnumerable<IGohResourceLoader> s_baseResourceDirectories = [
            new PakResourceLoader(),
            new FileSystemResourceLoader(),
            new ModResourceLoader()
        ];
        private IGohResourceLoader? _baseResourceLoader;
        private IGohResourceLoader? _currentResourceLoader;

        public IGohResourceLoader ResourceLoader => _currentResourceLoader ?? throw GohResourceLoadException.IsNotLoad();
        public GohResourceDirectory ResourceDirectory => ResourceLoader?.Root ?? throw GohResourceLoadException.IsNotLoad();
        public bool IsResourceLoaded => ResourceDirectory != null;

        public GohModResourceProvider ModResourceProvider { get; }

        public event EventHandler? ResourceUpdated;
        public event EventHandler? ResourceFullLoaded;

        public GohResourceProvider(GohModResourceProvider modResourceProvider) {
            ModResourceProvider = modResourceProvider;
        }

        public void OpenResources(string path) {
            _baseResourceLoader = _currentResourceLoader = GetResourceLoader(path) ?? throw GohResourceLoadException.IsNotGohResource(path);
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
            FullLoad();
            OnResourceUpdated();
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            string path = GohResourceLocations.GetLocationPathByName(location);

            return GetDirectory(path) ?? throw GohResourceLoadException.LocationNotFound(location, path);
        }

        public GohResourceDirectory GetResourceDirectory(GohResourceElement resourceElement) {
            string? path = resourceElement.GetDirectoryPath();
            GohResourceDirectory? resourceDirectory;

            if (path != null) {
                resourceDirectory = GetDirectory(path);
            } else {
                resourceDirectory = ResourceDirectory;
            }

            if (resourceDirectory == null || resourceDirectory.GetFile(resourceElement.Name) == null) {
                throw GohResourceLoadException.ElementNotInResource(resourceElement);
            }

            return resourceDirectory;
        }

        public GohResourceDirectory? GetDirectory(string path) {
            return ResourceDirectory.AlongPath(path);
        }

        public GohResourceFile? GetFile(string fullName) {
            string? path = Path.GetDirectoryName(fullName);
            GohResourceDirectory? directory = path != null ? GetDirectory(path) : ResourceDirectory;

            string name = Path.GetFileName(fullName);

            return directory?.GetFile(name);
        }

        public void FullLoad(Action<GohResourceElement>? elementLoad = null, CancellationToken? cancellationToken = null) {
            bool perdicate(GohResourceElement element) {
                cancellationToken?.ThrowIfCancellationRequested();
                elementLoad?.Invoke(element);
                return false;
            }

            ResourceDirectory.FindResourceElements(perdicate);
            OnResourceFullLoaded();
        }

        private IGohResourceLoader? GetResourceLoader(string path) {
            foreach (var resourceDirectory in s_baseResourceDirectories) {
                if (resourceDirectory.CheckRootPath(path)) {
                    return resourceDirectory;
                }
            }

            return null;
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnResourceFullLoaded() {
            ResourceFullLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
