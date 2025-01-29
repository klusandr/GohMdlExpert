using System.IO;
using System.Reflection;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class GohResourceProvider {
        private static readonly Dictionary<string, Type> s_fileTypes = new() {
            [".mdl"] = typeof(MdlFile),
            [".ply"] = typeof(PlyFile),
            [".mtl"] = typeof(MtlFile),
            [".dds"] = typeof(MaterialFile),
        };

        private static readonly IEnumerable<IGohResourceLoader> s_baseResourceDirectories = [
            //new DefaultResourceLoader(),
            new PakResourceLoader()
        ];

        private IGohResourceLoader? _baseResourceDirectory;

        public IGohResourceLoader BaseResourceDirectory => _baseResourceDirectory ?? throw GohResourcesException.DirectoryNotSpecified();
        public GohResourceDirectory ResourceDirectory => BaseResourceDirectory?.Root ?? throw GohResourcesException.DirectoryNotSpecified();
        public bool IsResourceLoaded => ResourceDirectory != null;

        public event EventHandler? ResourceUpdated;

        public GohResourceProvider() { }

        public void OpenResources(string path) {
            foreach (var resourceDirectory in s_baseResourceDirectories) {
                if (resourceDirectory.CheckBasePath(path)) {
                    _baseResourceDirectory = resourceDirectory;
                    _baseResourceDirectory.LoadData(path);
                    OnResourceUpdated();
                    return;
                }
            }

            throw GohResourcesException.IsNotGohResource(path);
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            return BaseResourceDirectory.GetLocationDirectory(location);
        }

        public GohResourceDirectory GetResourceDirectory(GohResourceElement resourceElement) {
            string? path = resourceElement.GetDirectoryPath();
            GohResourceDirectory? resourceDirectory;

            if (path != null) {
                resourceDirectory = BaseResourceDirectory.GetDirectory(path);
            } else {
                resourceDirectory = ResourceDirectory;
            }

            if (resourceDirectory == null || resourceDirectory.GetFile(resourceElement.Name) == null) {
                throw GohResourcesException.ElementNotInResource(resourceElement);
            }

            return resourceDirectory;
        }

        private void OnResourceUpdated() {
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
