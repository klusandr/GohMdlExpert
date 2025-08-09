using System.IO;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public interface IFileLoader {
        IGohResourceLoader ResourceLoader { get; }
        bool IsReadOnly { get; }
        Stream GetStream(string path);
        string GetAllText(string path);
        bool Exists(string path);
    }
}
