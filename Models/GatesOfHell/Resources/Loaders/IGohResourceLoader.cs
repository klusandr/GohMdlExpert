using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public interface IGohResourceLoader {
        GohResourceDirectory? Root { get; }
        bool CheckBasePath(string path);
        void LoadData(string path);
        GohResourceDirectory GetLocationDirectory(string location);
        GohResourceDirectory? GetDirectory(string path);
        GohResourceFile? GetFile(string fullName);
    }
}
