using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class PlyAggregateMtlFiles : IEnumerable<PlyAggregateMtlFile> {
        protected readonly Dictionary<string, PlyAggregateMtlFile> _files;

        public PlyFile PlyFile { get; }

        public IEnumerable<string> FilesNames => _files.Keys;

        public PlyAggregateMtlFile this[string key] {
            get {
                ResourceChecking.ThrowCheckPlyFileMeshTextureName(PlyFile, key);
                return _files[key];
            }
        }

        public PlyAggregateMtlFiles(PlyFile plyFile, GohFactionHumanskinResource humanskinResource, GohTextureProvider? textureProvider = null) {
            _files = [];
            PlyFile = plyFile;

            foreach (var mesh in plyFile.Data.Meshes) {
                _files.Add(mesh.TextureName, new PlyAggregateMtlFile(mesh.TextureName, plyFile, humanskinResource));
            }

            if (textureProvider != null) {
                foreach (var mtlTexture in _files.Values.SelectMany(a => a.Data)) {
                    textureProvider.SetTextureMaterialsFullPath(mtlTexture);
                }
            }
        }

        public Dictionary<string, MtlTextureCollection> GetMeshesTextures() {
            return new Dictionary<string, MtlTextureCollection>(_files.Select(kv => new KeyValuePair<string, MtlTextureCollection>(kv.Key, kv.Value.Data)));
        }

        public Dictionary<string, MtlTexture?> GetFirstMeshesTextures() {
            return new Dictionary<string, MtlTexture?>(_files.Select(kv => new KeyValuePair<string, MtlTexture?>(kv.Key, kv.Value.Data.FirstOrDefault())));
        }

        public IEnumerator<PlyAggregateMtlFile> GetEnumerator() {
            return _files.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
