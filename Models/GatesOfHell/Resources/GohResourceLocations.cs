using System.Reflection;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public static class GohResourceLocations {
        private static readonly Dictionary<string, PropertyInfo> s_locations;

        public static string Texture => @"texture";
        public static string TextureCommon => @"texture\common";
        public static string Humanskin => @"entity\humanskin";

        static GohResourceLocations() {
            s_locations = [];

            foreach (var property in typeof(GohResourceLocations).GetProperties()) {
                s_locations.Add(property.Name.ToLower(), property);
            }
        }

        public static string GetLocationPathByName(string name) {
            if (s_locations.TryGetValue(name.ToLower(), out var propety)) {
                return (string)propety.GetValue(null)!;
            } else {
                throw GohResourceLoadException.LocationNotDefined(name);
            }
        }
    }
}
