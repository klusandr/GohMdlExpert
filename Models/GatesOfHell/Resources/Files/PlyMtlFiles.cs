using System.Collections;
using System.Linq;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class PlyMtlFiles : IEnumerable<MtlFile> {
        protected readonly Dictionary<string, MtlFile> _files;

        public PlyFile PlyFile { get; }

        public IEnumerable<string> FilesNames => _files.Keys;

        public MtlFile this[string key]  {
            get {
                ResourceChecking.ThrowCheckPlyFileMeshTextureName(PlyFile, key);
                return _files[key];
            }
        }

        protected PlyMtlFiles(PlyFile plyFile, IEnumerable<MtlFile> mtlFiles, bool isVerified) {
            PlyFile = plyFile;
            _files = [];

            foreach (var mtlFile in mtlFiles) {
                if (!isVerified) {
                    ResourceChecking.ThrowCheckPlyFileMeshTextureName(plyFile, mtlFile.Name);
                }

                _files.Add(mtlFile.Name, mtlFile);
            }
        }

        public PlyMtlFiles(PlyFile plyFile, IEnumerable<MtlFile> mtlFiles) : this(plyFile, mtlFiles, false) { }

        public IEnumerator<MtlFile> GetEnumerator() {
            return _files.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
