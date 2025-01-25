using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Serialization;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class MdlModel {
        public ModelDataSerializer.ModelDataParameter Parameters { get; set; }
        public PlyFile[] PlyModel { get; private set; }
        public Dictionary<PlyFile, PlyFile[]> PlyModelLods { get; private set; }

        public MdlModel(ModelDataSerializer.ModelDataParameter parameters, IEnumerable<PlyFile> plyFiles, Dictionary<PlyFile, PlyFile[]> plyModelLods) {
            Parameters = parameters;
            PlyModel = plyFiles.ToArray();
            PlyModelLods = plyModelLods;
        }
    }
}
