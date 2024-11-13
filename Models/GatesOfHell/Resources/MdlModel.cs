using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class MdlModel {
        public PlyFile[] PlyModelFiles { get; private set; }
        public MtlFile[] Textures { get; private set; }

        public MdlModel(IEnumerable<PlyFile> plyFiles, IEnumerable<MtlFile> textures) {
            PlyModelFiles = plyFiles.ToArray();
            Textures = textures.ToArray();
        }
    }
}
