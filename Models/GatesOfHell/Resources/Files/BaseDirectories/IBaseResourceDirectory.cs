namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.BaseDirectories {
    public interface IBaseResourceDirectory {
        GohResourceDirectory? Root { get; }
        bool CheckBasePath(string path);
        void LoadData(string path);
        GohResourceDirectory GetLocationDirectory(string location);
        GohResourceDirectory? GetDirectory(string path);
        GohResourceFile? GetFile(string fullName);
    }
}
