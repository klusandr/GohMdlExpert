using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class ModResourceLoader : DefaultResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "resource"
        };

        public override bool CheckBasePath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceNeedDirectories.All((d) => directories.Contains(d));
        }

        public override void LoadData(string path) {
            if (!CheckBasePath(path)) {
                throw GohResourcesException.IsNotGohResource(path);
            }

            Root = new GohResourceDirectory(path) { Loader = new ModDirectoryLoader(path) };
        }
    }
}
