using GohMdlExpert.Models.GatesOfHell.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class ResourceLocations {
        private const string RESORCE_PATH_SIGNATURE = "|res|";

        private static ResourceLocations? s_instance;

        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        private string? _resourcePath;
        private readonly Dictionary<string, string> _locationsPaths = new() {
            ["textures"] = @"|res|\texture\common",
            ["ger_humanskin"] = @"|res|\entity\humanskin",
            ["base"] = "|res|"
        };

        public static ResourceLocations Instance => s_instance ??= new ResourceLocations();

        public string ResourcePath {
            get {
                return _resourcePath ?? throw new GohResourcesException("Resources path not specified.");
            }

            set {
                if (CheckGohResourceDirectory(value)) {
                    _resourcePath = value;
                } else {
                    throw new GohResourcesException("Specified directory is not GoH resources.");
                }
            }
        }

        private ResourceLocations() {
            
        }

        public string GetLocationPath(string location) {
            if (!_locationsPaths.TryGetValue(location, out string? path)) {
                throw new GohResourcesException("Resource location is not defined.");
            } else {
                if (path.Contains(RESORCE_PATH_SIGNATURE)) {
                    path = path.Replace(RESORCE_PATH_SIGNATURE, ResourcePath);
                }

                return path;
            }
        }

        public bool CheckGohResourceDirectory(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf(@"\") + 1)..]);
            return s_resourceNeedDirectories
                .All((d) => directories
                    .Contains(d)
                );
        }
    }
}
