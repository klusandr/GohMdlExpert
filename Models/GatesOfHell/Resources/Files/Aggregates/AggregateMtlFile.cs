using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates {
    public class AggregateMtlFile : GohResourceFile {
        private readonly PlyFile _plyFile;
        private readonly GohFactionHumanskinResource? _humanskinResource;

        public PlyFile PlyFile => _plyFile;

        public new MtlTextureCollection Data { get => (MtlTextureCollection)base.Data; set => base.Data = value; }

        public AggregateMtlFile(string name, PlyFile plyFile, GohFactionHumanskinResource HumanskinResource) : base(name) {
            _plyFile = plyFile;
            _humanskinResource = HumanskinResource;
        }

        public AggregateMtlFile(MtlFile mtlFile, PlyFile plyFile) : base(mtlFile.Name) {
            _plyFile = plyFile;
            _humanskinResource = null;
            Data = new MtlTextureCollection() { mtlFile.Data };
        }

        public override void LoadData() {
            try {
                if (_humanskinResource != null) {
                    Data = _humanskinResource.GetPlyMeshMtlTextures(PlyFile, Name);
                }
            } catch (InvalidOperationException) {
                throw new GohResourceFileException($"Error load aggregate .mtl file for {_plyFile.Name} file. Ply file don't contain mesh with the same name.", Name);
            }
        }
    }
}
