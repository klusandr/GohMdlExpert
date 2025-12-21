using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using DataList = System.Collections.Generic.IList<GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer.ModelDataParameter>;
using ModelDataParameter = GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer.ModelDataParameter;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files
{
    public class MdlFile : GohResourceFile {
        public static MdlSerializer? s_serializer;

        public static MdlSerializer Serializer => s_serializer ??= new MdlSerializer();

        public new MdlModel Data { get => (MdlModel)base.Data; set => base.Data = value; }

        public static string Extension => ".mdl";

        public MdlFile(string name, string? path = null, string? relativePathPoint = null)
            : base(name, path, relativePathPoint) { }

        public override string? GetExtension() {
            return Extension;
        }

        public override void LoadData() {
            var parameter = Serializer.Deserialize(ReadAllText());
            var plyFiles = new List<PlyFile>();
            var plyLodFiles = new Dictionary<PlyFile, PlyFile[]>();

            var plyLodModels = (IEnumerable<ModelDataParameter>)ModelDataSerializer.FindParameterByName(parameter, "skin")?.Data!;

            if (plyLodModels != null) {
                foreach (var plyLodModel in plyLodModels) {
                    if (plyLodModel.Type == MdlSerializer.MdlTypes.LODView.ToString()) {
                        var lodParameters = (IEnumerable<ModelDataParameter>)plyLodModel.Data!;

                        var plyModelParameter = lodParameters.First();
                        var lodModelsParameters = lodParameters.Skip(1);

                        var plyFile = new PlyFile((string)plyModelParameter.Data!, relativePathPoint: GetDirectoryPath()) { Loader = Loader };
                        var lodFiles = new List<PlyFile>();
                        plyFiles.Add(plyFile);


                        foreach (var lodParameter in lodModelsParameters) {
                            lodFiles.Add(new PlyFile((string)lodParameter.Data!, relativePathPoint: GetDirectoryPath()) { Loader = Loader });
                        }

                        plyLodFiles.TryAdd(plyFile, [.. lodFiles]);
                    } else if (plyLodModel.Type == MdlSerializer.MdlTypes.VolumeView.ToString()) {
                        plyFiles.Add(new PlyFile((string)plyLodModel.Data!, relativePathPoint: GetDirectoryPath()) { Loader = Loader });
                    }
                }

                Data = new MdlModel(parameter, plyFiles, plyLodFiles);
            } else {
                throw GohResourceFileException.InvalidFormat(this, "mdl data format");
            }
        }

        public override void SaveData() {
            if (Loader.IsReadOnly) {
                throw GohResourceSaveException.SaveReadOnlyFile(this);
            }

            var parameters = Data.Parameters;
            var skinParameter = new ModelDataParameter() {
                Type = MdlSerializer.MdlTypes.Bone.ToString(),
                Name = "skin"
            };

            var lodViews = new List<ModelDataParameter>();

            foreach (var plyFile in Data.PlyModels) {
                var volumeViews = new List<ModelDataParameter>() {
                    new(MdlSerializer.MdlTypes.VolumeView.ToString()) {
                        Data = SystemPath.Join(GohResourceLoading.GetRelativelyPath(GetDirectoryPath()!, plyFile.GetDirectoryPath()!), plyFile.Name) 
                    }
                };

                foreach (var plyLodFile in Data.PlyModelsLods[plyFile]) {
                    volumeViews.Add(new ModelDataParameter(MdlSerializer.MdlTypes.VolumeView.ToString()) {
                        Data = SystemPath.Join(GohResourceLoading.GetRelativelyPath(GetDirectoryPath()!, plyLodFile.GetDirectoryPath()!), plyLodFile.Name)
                    });
                }

                lodViews.Add(new ModelDataParameter(MdlSerializer.MdlTypes.LODView.ToString()) {
                    Data = volumeViews
                });
            }

            skinParameter.Data = lodViews;

            ((DataList)((DataList)parameters.Data!)[0].Data!)[13] = skinParameter;

            WriteAllText(Serializer.Serialize(Data.Parameters));
        }

        public override void UnloadData() {
            if (DataIsLoaded) {
                foreach (var plyFile in Data.PlyModels) {
                    plyFile.UnloadData();
                }

                foreach (var plyModelLods in Data.PlyModelsLods) {
                    foreach (var plyFile in plyModelLods.Value) {
                        plyFile.UnloadData();
                    }
                }
            }

            base.UnloadData();
        }
    }
}
