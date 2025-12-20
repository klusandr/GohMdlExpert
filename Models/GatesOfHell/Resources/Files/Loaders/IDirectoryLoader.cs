using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public interface IDirectoryLoader {
        IGohResourceLoader ResourceLoader { get; }
        bool Exists(string path);
        IEnumerable<GohResourceDirectory> GetDirectories(string path);
        IEnumerable<GohResourceFile> GetFiles(string path);
    }
}
