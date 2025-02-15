using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Extensions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class PakFileLoader : IFileLoader {
        private ZipArchive _archive;

        public bool IsReadOnly => true;

        public PakFileLoader(ZipArchive archive) {
            _archive = archive;
        }

        public bool Exists(string path) {
            return GetEntry(path) != null;
        }

        public string GetAllText(string path) {
            using var stream = GetStream(path);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer);

            return Encoding.UTF8.GetString(buffer);
        }

        public Stream GetStream(string path) {
            var entry = GetEntry(path) ?? throw GohResourceFileException.IsNotExists(path);
            
            return entry.Open();
        }


        private ZipArchiveEntry? GetEntry(string path) {
            string fullPath = _archive.GetArchiveFilePath(path).ToLower();

            return _archive.Entries.FirstOrDefault(e => e.FullName.ToLower() == fullPath);
            //return _archive.GetEntry(_archive.GetArchiveFilePath(path)) != null;
        }
    }
}
