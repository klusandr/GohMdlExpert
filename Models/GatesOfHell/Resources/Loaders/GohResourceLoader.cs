using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public abstract class GohResourceLoader : IGohResourceLoader {
        private static readonly Dictionary<string, string> s_locationsPaths = new() {
            ["texture"] = @"\texture\common",
            ["humanskins"] = @"\entity\humanskin",
            ["ger_humanskin"] = @"\entity\humanskin\[germans]",
            ["us_humanskin"] = @"\entity\humanskin\[united_states]",
            ["sov_humanskin"] = @"\entity\humanskin\[soviets]",
        };

        public abstract GohResourceDirectory? Root { get; protected set; }

        public abstract bool CheckBasePath(string path);

        public abstract void LoadData(string path);

        public virtual GohResourceDirectory? GetDirectory(string path) {
            if (Root == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            if (Path.IsPathFullyQualified(path)) {
                if (path.Contains(Root.GetFullPath())) {
                    path = path.Replace(Root.GetFullPath(), null);
                } else {
                    throw GohResourcesException.ElementNotInResource(path);
                }
            }

            return Root.AlongPath(path);
        }

        public virtual GohResourceDirectory GetLocationDirectory(string location) {
            if (!s_locationsPaths.TryGetValue(location, out string? path)) {
                throw GohResourcesException.LocationNotDefined(location);
            }

            return GetDirectory(path) ?? throw GohResourcesException.LocationNotFound(location, path);
        }

        public virtual GohResourceFile? GetFile(string fullName) {
            if (Root == null) {
                throw GohResourcesException.DirectoryNotSpecified();
            }

            string? path = Path.GetDirectoryName(fullName);
            GohResourceDirectory? directory;

            if (path != null) {
                try {
                    directory = GetDirectory(path);
                } catch (GohResourcesException) {
                    throw GohResourcesException.ElementNotInResource(fullName);
                }
            } else {
                directory = Root;
            }

            string name = Path.GetFileName(fullName);

            return directory?.GetFile(name);
        }
    }
}
