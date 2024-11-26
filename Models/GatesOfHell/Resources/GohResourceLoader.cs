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
    public class GohResourceLoader {
        private static GohResourceLoader? s_instance;

        private static Dictionary<string, Type> s_fileTypes = new() {
            [".mdl"] = typeof(MdlFile),
            [".ply"] = typeof(PlyFile),
            [".mtl"] = typeof(MtlFile),
        }; 

        private GohResourceLocations? _resourceLocations;
        private GohResourceDirectory? _resourceDictionary;
        private Dictionary<string, TextureFile> _textureFiles;

        public static GohResourceLoader Instance => s_instance ??= new GohResourceLoader();

        public GohResourceLocations ResourceLocations => _resourceLocations ??= GohResourceLocations.Instance;

        public GohResourceDirectory ResourceDictionary {
            get {
                if (_resourceDictionary == null) {
                    LoadResources();
                }

                return _resourceDictionary!;
            }
            private set { _resourceDictionary = value; }
            
        }

        public ICollection<TextureFile> LoadedTextureFiles => _textureFiles.Values;

        public GohResourceLoader() {
            _textureFiles = [];
        }

        public void LoadResources() {
            ResourceDictionary = new GohResourceDirectory(ResourceLocations.GetLocationFullPath("base"));
        }

        public GohResourceDirectory GetResourceDirectory(string location) {
            string locationPath = ResourceLocations.GetLocationPath(location);

            string[] directoryNames = locationPath.Split('\\', StringSplitOptions.RemoveEmptyEntries);

            GohResourceDirectory? findDirectory = ResourceDictionary;

            foreach (string directoryName in directoryNames) {
                findDirectory = findDirectory.GetDirectories().FirstOrDefault(d => d.Name == directoryName);

                if (findDirectory == null) {
                    break;
                }
            }

            return findDirectory ?? throw new GohResourcesException($"Resource path \"(GoHResource)\\{locationPath}\" is not find");
        }

        public GohResourceFile GetResourceFile(string fileName, string? path = null) {
            if (s_fileTypes.TryGetValue(Path.GetExtension(fileName), out var fileType)) {
                return (GohResourceFile)fileType.GetConstructors()[0].Invoke([fileName, path, null, null])!;
            } else {
                return new GohResourceFile(fileName, path);
            }
        }

        //TODO Поиск .mdl файлов по имени .ply без пути может вызвать выборку .mdl содержащих другие .ply файлы с идентичными именами, ну другими путями.
        public IEnumerable<MdlFile> GetPlyMdlFiles(PlyFile plyFile, GohResourceDirectory? currentDirectory = null) {
            currentDirectory ??= GetResourceDirectory("ger_humanskin");

            var mdlFiles = currentDirectory.GetFiles()
                .OfType<MdlFile>()
                .Where(f => f.GetAllText().Contains(plyFile.Name));

            foreach (var directory in currentDirectory.GetDirectories()) {
                var directoryMdlFiles = GetPlyMdlFiles(plyFile, directory);

                if (directoryMdlFiles != null) {
                    mdlFiles = mdlFiles.Concat(directoryMdlFiles);
                }
            }

            return mdlFiles;
        }

        public MtlTextureCollection GetPlyMeshMtlTextures(PlyFile plyFile, PlyModel.Mesh mesh) {
            var mdlFiles = GetPlyMdlFiles(plyFile);
            var mtlTextures = new MtlTextureCollection();

            foreach (var mdlFile in mdlFiles) {
                var directory = new GohResourceDirectory(mdlFile);

                foreach (var mtlFile in directory.GetFiles().OfType<MtlFile>()) {
                    if (mtlFile.Name == mesh.TextureName) {
                        mtlTextures.Add(mtlFile.Data);
                    }
                }
            }

            return mtlTextures;
        }
    }
}
