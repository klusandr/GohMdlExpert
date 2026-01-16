using GohMdlExpert.Models.GatesOfHell.Resources.Data;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates {
    public class AggregateMtlFile : GohResourceFile {
        private readonly Func<MtlTextureCollection>? _loadData;

        public new MtlTextureCollection Data { get => (MtlTextureCollection)base.Data; set => base.Data = value; }

        public AggregateMtlFile(string name, MtlTextureCollection mtlTextures) : base(name) {
            Data = mtlTextures;
        }

        public AggregateMtlFile(string name, Func<MtlTextureCollection> loadData) : base(name) {
            _loadData = loadData;
        }

        public AggregateMtlFile(MtlFile mtlFile) : base(mtlFile.Name) {
            Data = [mtlFile.Data];
        }

        public override void LoadData() {
            if (_loadData != null) {
                Data = _loadData();
            }
        }
    }
}
