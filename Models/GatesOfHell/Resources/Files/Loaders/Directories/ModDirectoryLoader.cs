using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories {
    public class ModDirectoryLoader : FileSystemDirectoryLoader {
        public ModDirectoryLoader(ModResourceLoader resourceLoader) : base(resourceLoader, new ModFileLoader(resourceLoader)) { }
    }
}
