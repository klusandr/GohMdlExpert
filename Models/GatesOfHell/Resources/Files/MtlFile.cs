using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class MtlFile : GohResourceFile {
        public static MtlSerializer? s_serializer;

        protected static MtlSerializer Serializer => s_serializer ??= new MtlSerializer();

        public override string? Extension => ".mtl";

        public new MtlTexture Data { get => (MtlTexture)base.Data; set => base.Data = value; }

        public MtlFile(string name, string? path = null, string? relativePathPoint = null) 
            : base(name, path, relativePathPoint) { }

        public override void LoadData() {
            var parameters = Serializer.Deserialize(GetAllText());

            string diffusePath = (string)ModelDataSerializer.FindParameter(parameters, "Diffuse")?.Data!;

            Data = new MtlTexture(new TextureFile(diffusePath));
        }
    }
}
