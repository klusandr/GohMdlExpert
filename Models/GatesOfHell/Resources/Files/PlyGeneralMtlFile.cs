using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class PlyGeneralMtlFile : MtlFile {
        private MtlTextureCollection? _data;
        private readonly PlyFile _plyFile;

        public PlyFile PlyFile => _plyFile;

        public new MtlTextureCollection Data { 
            get {
                LoadData();
                return _data!;
            }
            set => _data = value;
        }

        public PlyGeneralMtlFile(string name, PlyFile plyFile) : base(name, null, null, null) {
            _plyFile = plyFile;
        }

        public override void LoadData() {
            try {
                var plyMash = _plyFile.Data.Meshes.First(x => x.TextureName == Name);

                Data = Loader.GetPlyMeshMtlTextures(PlyFile, plyMash);
            } catch (InvalidOperationException e) {
                throw new GohResourceFileException($"Error load general .mtl file for {_plyFile.Name} file.", Name);
            }
        }
    }
}
