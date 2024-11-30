using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class PlyAggregateMtlFile : GohResourceFile {
        private readonly PlyFile _plyFile;
        private readonly GohFactionHumanskinResource _humanskinResource;

        public PlyFile PlyFile => _plyFile;

        public new MtlTextureCollection Data { get => (MtlTextureCollection)base.Data; set => base.Data = value; }

        public PlyAggregateMtlFile(string name, PlyFile plyFile, GohFactionHumanskinResource HumanskinResource) : base(name) {
            _plyFile = plyFile;
            _humanskinResource = HumanskinResource;
        }

        public override void LoadData() {
            try {
                Data = _humanskinResource.GetPlyMeshMtlTextures(PlyFile, Name);
            } catch (InvalidOperationException) {
                throw new GohResourceFileException($"Error load aggregate .mtl file for {_plyFile.Name} file. Ply file don't contain mesh with the same name.", Name);
            }
        }
    }
}
