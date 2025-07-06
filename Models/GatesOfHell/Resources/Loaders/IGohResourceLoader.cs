using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public interface IGohResourceLoader {
        GohResourceDirectory? Root { get; }
        bool CheckRootPath(string path);
        void LoadData(string path);
    }
}
