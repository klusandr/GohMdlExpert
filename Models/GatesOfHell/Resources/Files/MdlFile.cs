using System.IO;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using DataList = System.Collections.Generic.IList<GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer.ModelDataParameter>;
using ModelDataParameter = GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer.ModelDataParameter;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class MdlFile : GohResourceFile {
        public static MdlSerializer? s_serializer;

        public static MdlSerializer Serializer => s_serializer ??= new MdlSerializer();

        public new MdlModel Data { get => (MdlModel)base.Data; set => base.Data = value; }

        public static string? Extension => ".mdl";

        public MdlFile(string name, string? path = null, string? relativePathPoint = null)
            : base(name, path, relativePathPoint) { }

        public override string? GetExtension() {
            return Extension;
        }

        public override void LoadData() {
            var parameter = Serializer.Deserialize(GetAllText());
            var plyFiles = new List<PlyFile>();
            var plyLodFiles = new Dictionary<PlyFile, PlyFile[]>();

            var plyLodModels = (IEnumerable<ModelDataParameter>)ModelDataSerializer.FindParameterByName(parameter, "skin")?.Data!;

            foreach (var plyLodModel in plyLodModels) {
                var lodParameters = (IEnumerable<ModelDataParameter>)plyLodModel.Data!;

                var plyModelParameter = lodParameters.First();
                var lodModelsParameters = lodParameters.Skip(1);

                var plyFile = new PlyFile(RelativePathRemove((string)plyModelParameter.Data!));
                var lodFiles = new List<PlyFile>();
                plyFiles.Add(plyFile);


                foreach (var lodParameter in lodModelsParameters) {
                    lodFiles.Add(new PlyFile(RelativePathRemove((string)lodParameter.Data!)));
                }

                plyLodFiles.Add(plyFile, [.. lodFiles]);
            }

            Data = new MdlModel(parameter, plyFiles, plyLodFiles);
        }

        public override void SaveData() {
            var parameters = Data.Parameters;
            var skinParameter = new ModelDataParameter() {
                Type = MdlSerializer.MdlTypes.Bone.ToString(),
                Name = "skin"
            };

            var lodViews = new List<ModelDataParameter>();

            foreach (var plyFile in Data.PlyModel) {
                var volumeViews = new List<ModelDataParameter>() {
                    new(MdlSerializer.MdlTypes.VolumeView.ToString()) {
                        Data = plyFile.GetFullPath()
                    }
                };

                foreach (var plyLodFile in Data.PlyModelLods[plyFile]) {
                    volumeViews.Add(new ModelDataParameter(MdlSerializer.MdlTypes.VolumeView.ToString()) {
                        Data = plyLodFile.GetFullPath()
                    });
                }

                lodViews.Add(new ModelDataParameter(MdlSerializer.MdlTypes.LODView.ToString()) {
                    Data = volumeViews
                });
            }

            skinParameter.Data = lodViews;

            ((DataList)((DataList)parameters.Data!)[0].Data!)[13] = skinParameter;

            var str = Serializer.Serialize(Data.Parameters);

            using var stream = new StreamWriter(GetFullPath());

            stream.Write(str);
        }

        private string RelativePathRemove(string relativePath) {
            return relativePath.Replace("../", null);
        }
    }
}
