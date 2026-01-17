using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public interface IGohResourceLoader {
        GohResourceDirectory? Root { get; }
    }
}
