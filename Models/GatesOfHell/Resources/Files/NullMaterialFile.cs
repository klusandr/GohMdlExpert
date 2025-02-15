using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class NullMaterialFile : MaterialFile {
        public NullMaterialFile() : base(string.Empty) {}

        public override string? GetExtension() {
            return null;
        }

        protected override DiffuseMaterial GetMaterial() {
            return ResourceConverts.GetRandomTexture();
        }
    }
}
