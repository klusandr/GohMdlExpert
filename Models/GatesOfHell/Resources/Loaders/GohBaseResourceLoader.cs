using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public abstract class GohBaseResourceLoader : IGohResourceLoader {
        public abstract GohResourceDirectory Root { get; protected set; }

        public abstract bool CheckRootPath(string path);

        public abstract void LoadData(string path);
    }
}
