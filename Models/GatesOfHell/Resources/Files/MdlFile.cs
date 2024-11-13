using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDataParameter = GohMdlExpert.Models.GatesOfHell.Serialization.ModelDataSerializer.ModelDataParameter;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class MdlFile : GohResourceFile {
        public static MdlSerializer? s_serializer;

        public static MdlSerializer Serializer => s_serializer ??= new MdlSerializer();

        public new MdlModel? Data { get => (MdlModel?)base.Data; set => base.Data = value; }

        public override string? Extension => ".mdl";

        public MdlFile(string name, string? path = null, string? relativePathPoint = null, string? location = null) 
            : base(name, path, relativePathPoint, location) { }

        public override void LoadData() {
            var mdlParameters = Serializer.Deserialize(GetAllText());
            var plyFiles = new List<PlyFile>();
            var textureNames = new List<string>();

            var plyFilesParameters = (IEnumerable<ModelDataParameter>)ModelDataSerializer.FindParameter(mdlParameters, MdlSerializer.MdlTypes.Bone.ToString(), "skin")?.Data!;

            foreach (var plyFilesParameter in plyFilesParameters) {
                foreach (var plyFileName in ((IEnumerable<ModelDataParameter>)plyFilesParameter.Data!).Select(p => (string)p.Data!)) {
                    var plyFile = new PlyFile(plyFileName, relativePathPoint: Path);
                    textureNames.AddRange(plyFile.Data!.Meshes!.Select(m => m.TextureFileName));
                    plyFiles.Add(plyFile);
                }
            }

            var textures = textureNames.Distinct().Select(t => new MtlFile(t, Path));

            Data = new MdlModel(plyFiles, textures);
        }
    }
}
