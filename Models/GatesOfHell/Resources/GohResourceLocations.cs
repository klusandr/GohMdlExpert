using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohResourceLocations {
        public const string HUMANSKIN_SOURCE_DIRECTORY_NAME_REG = @"\[.*_source\]";

        private readonly Dictionary<string, string> _locationsPaths = new() {
            ["texture"] = @"\texture\common",
            ["ger_humanskin"] = @"\entity\humanskin\[germans]",
            ["us_humanskin"] = @"\entity\humanskin\[united_states]",
            ["sov_humanskin"] = @"\entity\humanskin\[soviets]",
            ["ger_humanskin_source"] = @"\entity\humanskin\[germans]\[ger_source]",
            ["base"] = ""
        };

        private readonly Dictionary<string, string[]> _aggregateLocationsPaths = new() {
            ["source"] = ["ger_humanskin_source"],
            ["humanskin_root"] = ["ger_humanskin"]
        };

        public GohResourceLocations() { }

        public string GetLocationPath(string location) {
            if (!_locationsPaths.TryGetValue(location, out string? path)) {
                throw GohResourcesException.LocationNotDefined(location);
            } else {
                return path;
            }
        }

        public IEnumerable<string> GetAggregateLocationPaths(string aggregateLocations) {
            if (_aggregateLocationsPaths.TryGetValue(aggregateLocations, out string[]? locations)) {
                var paths = new List<string>();

                foreach (var location in locations) {
                    if (!_locationsPaths.TryGetValue(location, out string? path)) {
                        path = location;
                    }

                    paths.Add(path);
                }

                return paths;
            } else {
                throw GohResourcesException.AggregateLocationNotDefined(aggregateLocations);
            }
        }
    }
}
