using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class PlyFile : GohResourceFile {
        private static PlySerializer? s_serializer;

        private static PlySerializer Serializer => s_serializer ??= new PlySerializer();

        public new PlyModel Data { get => (PlyModel)base.Data; set => base.Data = value; }

        public override string? Extension => ".ply";

        public PlyFile(string name, string? path = null, string? relativePathPoint = null) 
            : base(name, path, relativePathPoint) { }

        public override void LoadData() {
            Data = Serializer.Deserialize(GetStream());
        }
    }
}
