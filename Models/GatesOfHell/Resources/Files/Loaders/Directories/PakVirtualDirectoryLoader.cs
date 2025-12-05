using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories {
    public class PakVirtualDirectoryLoader : IDirectoryLoader {
        private readonly List<GohResourceDirectory> _resourceDirectories;

        public IGohResourceLoader ResourceLoader { get; }

        public PakVirtualDirectoryLoader(PakResourceLoader resourceLoader) {
            _resourceDirectories = [];
            ResourceLoader = resourceLoader;
        }

        public void AddPakDirectory(GohResourceDirectory resourceDirectory) {
            _resourceDirectories.Add(resourceDirectory);
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            return [.. _resourceDirectories];
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            return [];
        }
    }
}
