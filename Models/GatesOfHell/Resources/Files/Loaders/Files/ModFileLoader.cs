using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files {
    class ModFileLoader : FileSystemFileLoader {
        public override bool IsReadOnly => true;
        public ModFileLoader(ModResourceLoader resourceLoader) : base(resourceLoader) { }
    }
}
