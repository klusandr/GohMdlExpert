using System.IO;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class ModResourceLoader : FileSystemResourceLoader {
        private static readonly string[] s_resourceNeedDirectories = {
            "entity"
        };

        public override bool CheckRootPath(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceNeedDirectories.All((d) => directories.Contains(d));
        }
    }
}
