using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class ModDirectoryLoader : DefaultDirectoryLoader {
        private readonly string _path;

        public ModDirectoryLoader(string path) {
            _path = path;
        }

        public override IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            return base.GetDirectories(GetFullPath(path));
        }

        public override IEnumerable<GohResourceFile> GetFiles(string path) {
            return base.GetFiles(GetFullPath(path));
        }

        private string GetFullPath(string path) {
            return Path.IsPathRooted(path) ? path : Path.Join(_path, path.Contains(GohResourceLoading.ResourceDirectoryName) ? null : GohResourceLoading.ResourceDirectoryName, path);
        }
    }
}
