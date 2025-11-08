using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class ModResourceLoader : FileSystemResourceLoader {
        public ModResourceLoader(string path) : base(Path.Combine(path, GohResourceLoading.ResourceDirectoryName)) { }
    }
}
