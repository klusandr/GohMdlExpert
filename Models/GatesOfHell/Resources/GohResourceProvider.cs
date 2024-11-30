using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohResourceProvider {
        private static readonly Dictionary<string, Type> s_fileTypes = new() {
            [".mdl"] = typeof(MdlFile),
            [".ply"] = typeof(PlyFile),
            [".mtl"] = typeof(MtlFile),
        };

        private static readonly string[] s_resourceNeedDirectories = {
            "entity", "texture"
        };

        public GohResourceLocations ResourceLocations { get; }

        public GohResourceDirectory? ResourceDictionary { get; private set; }
        public bool IsResourceLoaded => ResourceDictionary != null;


        public event EventHandler? ResourceUpdated;

        public GohResourceProvider(GohResourceLocations resourceLocations) {
            ResourceLocations = resourceLocations;
        }

        public void OpenResources(string path) {
            if (!CheckGohResourceDirectory(path)) {
                throw new GohResourcesException($"{path} is not GoH resource directory.");
            }

            ResourceDictionary = new GohResourceDirectory(path);
            ResourceUpdated?.Invoke(this, EventArgs.Empty);
        }

        public GohResourceDirectory GetLocationDirectory(string location) {
            string locationPath = ResourceLocations.GetLocationPath(location);

            if (ResourceDictionary == null) {
                throw new GohResourcesException($"Resource directory is not specified.");
            }

            var findDirectory = ResourceDictionary.AlongPath(locationPath);

            return findDirectory ?? throw new GohResourcesException($"Resource path \"(GoHResource)\\{locationPath}\" is not find");
        }

        public static GohResourceFile GetResourceFile(string fileName, string? path = null) {
            if (s_fileTypes.TryGetValue(Path.GetExtension(fileName), out var fileType)) {
                return (GohResourceFile)fileType.GetConstructors()[0].Invoke([fileName, path, null])!;
            } else {
                return new GohResourceFile(fileName, path);
            }
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
