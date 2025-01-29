using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public interface IDirectoryLoader {
        IFileLoader FileLoader { get; }
        IEnumerable<GohResourceDirectory> GetDirectories(string path);
        IEnumerable<GohResourceFile> GetFiles(string path);
    }
}
