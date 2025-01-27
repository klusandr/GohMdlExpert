using System.IO;
using System.Text;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class DefaultFileLoader : IFileLoader {
        public bool IsReadOnly => false;

        public bool Exists(string path) {
            return File.Exists(path);
        }

        public Stream GetStream(string path) {
            if (!Exists(path)) {
                throw GohResourceFileException.IsNotExists(path);
            }

            return new FileStream(path, FileMode.Open);
        }

        public string GetAllText(string path) {
            using var stream = GetStream(path);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
