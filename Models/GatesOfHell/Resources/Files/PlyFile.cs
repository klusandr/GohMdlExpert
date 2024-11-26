using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class PlyFile : GohResourceFile {
        public static PlySerializer? s_serializer;
        private Dictionary<string, PlyGeneralMtlFile>? _textures;

        public static PlySerializer Serializer => s_serializer ??= new PlySerializer();

        public new PlyModel Data { get => (PlyModel)base.Data; set => base.Data = value; }

        public Dictionary<string, PlyGeneralMtlFile> Textures {
            get {
                LoadTextures();
                return _textures!;
            }
        }

        public override string? Extension => ".ply";

        public PlyFile(string name, string? path = null, string? relativePathPoint = null, string? location = null) 
            : base(name, path, relativePathPoint, location) { }

        public override void LoadData() {
            Data = Serializer.Deserialize(GetStream());
        }

        public void LoadTextures() {
            var meshes = Data.Meshes;

            _textures = [];

            foreach (var mesh in meshes) {
                _textures.Add(mesh.TextureName, new PlyGeneralMtlFile(mesh.TextureName, this));
            }
        }
    }
}
