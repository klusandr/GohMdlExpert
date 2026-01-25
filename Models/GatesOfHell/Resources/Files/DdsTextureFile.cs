using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    internal class DdsTextureFile : MaterialFile {
        public static string Extension => ".dds";

        public DdsTextureFile(string name, string? path = null, string? relativePathPoint = null) : base(name, Extension, path, relativePathPoint) { }

        public override string? GetExtension() {
            return Extension;
        }
    }
}
