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
        public const string HUMANSKIN_SOURCE_DIRECTORY_NAME = "[[*_source]]";

        private string? _resourcePath;
        private readonly Dictionary<string, string> _locationsPaths = new() {
            ["texture"] = @"\texture\common",
            ["ger_humanskin"] = @"\entity\humanskin\[germans]",
            ["base"] = ""
        };

        public GohResourceLocations() { }

        public string GetLocationPath(string location) {
            if (!_locationsPaths.TryGetValue(location, out string? path)) {
                throw new GohResourcesException($"Resource location \"{location}\" is not defined.");
            } else {
                return path;
            }
        }
    }
}
