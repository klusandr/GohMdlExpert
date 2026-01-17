using System.IO;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class ModResourceLoader : FileSystemResourceLoader {

        public ModResourceLoader(string path) : base(Path.Combine(path, GohResourceLoading.ResourceDirectoryName)) { }
    }
}
