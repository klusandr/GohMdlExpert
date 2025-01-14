using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Exceptions {
    public class GohResourcesException : GohException {
        private static readonly string s_message = "Loading Gates of hell resources error.";

        public GohResourcesException(string? message = null, Exception? inner = null) : base(GetFullErrorMessage(message), inner) { }

        public static GohResourcesException IsNotGohResource(string path) {
            return new GohResourcesException($"\"{path}\" is not GoH resource directory");
        } 

        public static GohResourcesException DirectoryNotSpecified() {
            return new GohResourcesException($"Resource directory is not specified.");
        }

        public static GohResourcesException LocationNotDefined(string location) {
            return new GohResourcesException($"Resource location \"{location}\" is not defined.");
        }

        internal static GohResourcesException AggregateLocationNotDefined(string aggregateLocations) {
            return new GohResourcesException($"Resource aggregate location \"{aggregateLocations}\" is not defined.");
        }

        public static GohResourcesException LocationNotFound(string location, string path) {
            return new GohResourcesException($"Resource location \"{location}\" on \"{path}\" not found.");
        }

        public static GohResourcesException PathIsNull(GohResourceElement resourceElement) {
            return new GohResourcesException($"Resource element \"{resourceElement.Name}\" path has be null.");
        }

        public static GohResourcesException ElementNotInResource(GohResourceElement resourceElement) {
            return new GohResourcesException($"Resource element \"{resourceElement.Name}\" is not in resource folder or subfolders.");
        }

        private static string GetFullErrorMessage(string? message = null) {
            if (message == null) {
                return s_message;
            } else {
                return s_message + " " + message;
            }
        }
    }
}
