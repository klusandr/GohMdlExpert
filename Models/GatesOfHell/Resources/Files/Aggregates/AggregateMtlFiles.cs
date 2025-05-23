﻿using System.Collections;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates
{
    public class AggregateMtlFiles : IEnumerable<AggregateMtlFile> {
        protected readonly Dictionary<string, AggregateMtlFile> _files;

        public PlyFile PlyFile { get; }

        public IEnumerable<string> FilesNames => _files.Keys;

        public AggregateMtlFile this[string key] {
            get {
                ResourceChecking.ThrowCheckPlyFileMeshTextureName(PlyFile, key);
                return _files[key];
            }
        }

        public AggregateMtlFiles(PlyFile plyFile, GohFactionHumanskinResource humanskinResource, GohTextureProvider? textureProvider = null) {
            _files = [];
            PlyFile = plyFile;

            foreach (var mesh in plyFile.Data.Meshes) {
                _files.Add(mesh.TextureName, new AggregateMtlFile(mesh.TextureName, plyFile, humanskinResource));
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

        public IEnumerator<AggregateMtlFile> GetEnumerator() {
            return _files.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
