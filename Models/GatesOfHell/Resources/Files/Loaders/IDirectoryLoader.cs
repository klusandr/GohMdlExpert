namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public interface IDirectoryLoader {
        IEnumerable<GohResourceDirectory> GetDirectories(string path);
        IEnumerable<GohResourceFile> GetFiles(string path);
    }
}
