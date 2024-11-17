using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class MdlModel {
        public ModelDataSerializer.ModelDataParameter[] Parameters { get; set; }
        public PlyFile[] PlyModelFiles { get; private set; }
        public MtlFile[] Textures { get; private set; }

        public MdlModel(IEnumerable<ModelDataSerializer.ModelDataParameter> parameters, IEnumerable<PlyFile> plyFiles, IEnumerable<MtlFile> textures) {
            Parameters = parameters.ToArray();
            PlyModelFiles = plyFiles.ToArray();
            Textures = textures.ToArray();
        }
    }
}
