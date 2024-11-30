using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class MtlTexture(MaterialFile diffuse) {
        public MaterialFile Diffuse { get; set; } = diffuse;
        public MaterialFile? Bump { get; set; }
        public MaterialFile? Specular { get; set; }
        public Colors? Color { get; set; }

        public override bool Equals(object? obj) {
            if (obj == null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj is MtlTexture mtl) {
                return Diffuse.Equals(mtl.Diffuse) 
                    && (Bump?.Equals(mtl.Bump) ?? mtl.Bump == null)
                    && (Specular?.Equals(mtl.Specular) ?? mtl.Specular == null)
                    && (Color?.Equals(mtl.Color) ?? mtl.Color == null);
            }
            
            return false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
