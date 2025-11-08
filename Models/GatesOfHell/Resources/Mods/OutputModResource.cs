using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class OutputModResource {
        public string Name { get; }
        public string Path { get; }
        public ModResourceLoader ResourceLoader { get; }

        public OutputModResource(string name, string path) {
            Name = name;
            Path = path;
            ResourceLoader = new ModResourceLoader(path);
        }

        public OutputModResource(string path) : this(SystemPath.GetFileName(path), path) { }
    }
}
