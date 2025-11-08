using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class ModResource {
        public string Path { get; }
        public bool IsLoaded { get; set; }
        public ModInfo? ModInfo { get; private set; }
        public ModResourceLoader ResourceLoader { get; private set; }
        public bool IsLoad => ResourceLoader != null;

        public ModResource(string path) {
            Path = path;
            ResourceLoader = new ModResourceLoader(path);
        }

        public void Load() {
            string modInfoPath = SystemPath.Join(Path, ModInfo.FileName);

            if (File.Exists(modInfoPath)) {
                try {
                    ModInfo = ModInfo.Parse(File.ReadAllText(modInfoPath));
                } catch (Exception) { }
            }

            IsLoaded = true;
        }
    }
}
