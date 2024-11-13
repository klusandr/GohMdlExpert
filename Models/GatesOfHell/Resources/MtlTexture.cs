using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class MtlTexture
    {
        public TextureFile? Diffuse { get; set; }
        public TextureFile? Bump { get; set; }
        public TextureFile? Specular { get; set; }
        public Colors? Color { get; set; }
    }
}
