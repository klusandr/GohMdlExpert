using System.IO;
using System.Reflection;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using Windows.ApplicationModel.Resources;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class GohResourceProvider {
        private IGohResourceLoader? _baseResourceLoader;
        private IGohResourceLoader? _outputModeResourceLoader;
        private IGohResourceLoader? _currentResourceLoader;

        public IGohResourceLoader ResourceLoader => _currentResourceLoader ?? throw GohResourceLoadException.IsNotLoad();
        public GohResourceDirectory ResourceDirectory => ResourceLoader?.Root ?? throw GohResourceLoadException.IsNotLoad();
        public bool IsResourceLoaded => ResourceDirectory != null;

        public GohOutputModProvider OutputModProvider { get; }
        public GohModResourceProvider ModResourceProvider { get; }

        public event EventHandler? ResourceUpdated;
        public event EventHandler? ResourceFullLoaded;

        public GohResourceProvider(GohOutputModProvider outputModProvider, GohModResourceProvider modResourceProvider) {
            OutputModProvider = outputModProvider;
            ModResourceProvider = modResourceProvider;

            OutputModProvider.ModUpdate += OutputModUpdateHandler;
        }

        private void OutputModUpdateHandler(object? sender, EventArgs e) {
            _outputModeResourceLoader = OutputModProvider.Mod.ResourceLoader;

            LoadModes();
        }

        public void LoadGameResource(string path) {
            _baseResourceLoader = _currentResourceLoader = new PakResourceLoader(path);
            OnResourceUpdated();
        }

        public void OpenResource(string path) {
            _baseResourceLoader = _currentResourceLoader = new FileSystemResourceLoader(path);
            OnResourceUpdated();
        }

        public void LoadModes() {
            if (_baseResourceLoader == null) {
                return;
            }

            var resourceLoaders = new List<IGohResourceLoader>() { _baseResourceLoader };

            if (_outputModeResourceLoader != null) {
                resourceLoaders.Add(_outputModeResourceLoader);
            }

            resourceLoaders.AddRange(ModResourceProvider.Mods.Where(m => m.IsLoaded && m.IsLoad).Select(m => m.ResourceLoader));

            if (resourceLoaders.Count > 1) {
                _currentResourceLoader = new AggregateResourceLoader([.. resourceLoaders]);
            } else {
                _currentResourceLoader = _baseResourceLoader;
            }

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

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnResourceFullLoaded() {
            ResourceFullLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
