using System.IO;
using System.IO.Compression;
using System.Text;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class PakFileLoader : IFileLoader {
        private ZipArchive _archive;

        public bool IsReadOnly => true;

        public PakFileLoader() {
            _archive = ZipFile.OpenRead(@"F:\Steam Game\steamapps\common\Call to Arms - Gates of Hell\resource\entity\humanskin.pak");
        }

        public bool Exists(string path) {
            path = path.Replace("\\", "/");
            return _archive.GetEntry(path) != null;
        }

        public string GetAllText(string path) {
            using var stream = GetStream(path);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer);

            return Encoding.UTF8.GetString(buffer);
        }

        public Stream GetStream(string path) {
            path = path.Replace("\\", "/");
            var entry = _archive.GetEntry(path) ?? throw GohResourceFileException.IsNotExists(path);

            return entry.Open();
        }

        
    }
}
