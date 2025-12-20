using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Serialization;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Data {
    public class MdlModel {
        public ModelDataSerializer.ModelDataParameter Parameters { get; set; }
        public PlyFile[] PlyModels { get; set; }
        public Dictionary<PlyFile, PlyFile[]> PlyModelsLods { get; set; }

        public MdlModel(ModelDataSerializer.ModelDataParameter parameters, IEnumerable<PlyFile> plyFiles, Dictionary<PlyFile, PlyFile[]> plyModelLods) {
            Parameters = parameters;
            PlyModels = [.. plyFiles];
            PlyModelsLods = plyModelLods;
        }
    }
}
