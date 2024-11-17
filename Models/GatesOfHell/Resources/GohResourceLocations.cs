using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohResourceLocations {
        private static GohResourceLocations? s_instance;

        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        private string? _resourcePath;
        private readonly Dictionary<string, string> _locationsPaths = new() {
            ["textures"] = @"\texture\common",
            ["ger_humanskin"] = @"\entity\humanskin\[germans]",
            ["ger_humanskin_source"] = @"\entity\humanskin\[germans]\[ger_source]",
            ["base"] = ""
        };

        public static GohResourceLocations Instance => s_instance ??= new GohResourceLocations();

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

        private GohResourceLocations() {
            
        }

        public string GetLocationFullPath(string location) {
            return GetLocationPath(location).Insert(0, ResourcePath);
        }

        public string GetLocationPath(string location) {
            if (!_locationsPaths.TryGetValue(location, out string? path)) {
                throw new GohResourcesException($"Resource location \"{location}\" is not defined.");
            } else {
                return path;
            }
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            return new GohResourceDirectory(GetLocationFullPath(location));
        }

        private static bool CheckGohResourceDirectory(string path) {
            var directories = Directory.GetDirectories(path).Select(d => d[(d.LastIndexOf('\\') + 1)..]);
            return s_resourceNeedDirectories
                .All((d) => directories
                    .Contains(d)
                );
        }
    }
}
