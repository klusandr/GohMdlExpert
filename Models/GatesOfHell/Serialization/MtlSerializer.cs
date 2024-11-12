using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Serialization {
    public class MtlSerializer : ModelDataSerializer {
        public MtlSerializer() {
            AddType("MaterialBump", "material bump");
            AddType("Diffuse", "diffuse");
            AddType("Bump", "bump");
            AddType("Specular", "specular");
            AddType("Color", "color");
            AddType("Blend", "blend");
        }
    }
}
