using System.IO;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public interface IFileLoader {
        bool IsReadOnly { get; }
        Stream GetStream(string path);
        string GetAllText(string path);
        bool Exists(string path);
    }
}
