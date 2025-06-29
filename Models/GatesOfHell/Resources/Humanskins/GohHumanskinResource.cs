using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Models.GatesOfHell.Сaches;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Humanskins {
    public class GohHumanskinResource : IGohHumanskinResource {
        private readonly GohResourceProvider _resourceProvider;
        private readonly GohCacheProvider _cacheProvider;

        public GohResourceDirectory Root { get; }
        public GohResourceDirectory? Source { get; private set; }

        public GohHumanskinResource(GohResourceDirectory humanskinsDirectory, GohResourceProvider resourceProvider, GohCacheProvider cacheProvider) {
            Root = humanskinsDirectory;
            _resourceProvider = resourceProvider;
            _cacheProvider = cacheProvider;
            Load();
        }

        public void Load() {
            static bool predicant(GohResourceElement element) {
                if (element is GohResourceDirectory directory) {
                    if (directory.GetFiles().Any(f => f is PlyFile)) {
                        return true;
                    }
                }

                return false;
            };

            var diretories = Root.FindResourceElements(predicant).Cast<GohResourceDirectory>().OrderBy((d) => d.GetFullPath());

            var sourceDirectory = new GohResourceVirtualDirectory(Root);

            foreach (var directory in diretories) {
                var path = directory.GetDirectoryPath();

                if (path != null) {
                    string localPath = path.Replace(Root.GetFullPath(), null);
                    var pathDirectory = sourceDirectory.AlongPathOrCreate(localPath);

                    var thatDirectory = pathDirectory.GetDirectory(directory.Name);

                    if (thatDirectory == null) {
                        pathDirectory.Items.Add(new GohResourceVirtualDirectory(directory.GetFullPath()) {
                            Items = [.. directory.GetFiles()]
                        });
                    } else {
                        thatDirectory.Items.AddRange(directory.GetFiles());
                    }
                }
            }

            Source = sourceDirectory;
        }

        public IEnumerable<MdlFile> GetPlyMdlFiles(PlyFile plyFile) {
            return Root
                .FindResourceElements<MdlFile>()
                .Where(m => m.GetAllText().Contains(plyFile.Name));
        }

        public IEnumerable<AggregateMtlFile> GetPlyAggregateMtlFiles(PlyFile plyFile) {
            var mtlFiles = new List<AggregateMtlFile>();

            foreach (var mesh in plyFile.Data.Meshes) {
                mtlFiles.Add(new AggregateMtlFile(mesh.TextureName, GetPlyMeshMtlTextures(plyFile, mesh.TextureName)));
            }

            return mtlFiles;
        }

        public MtlTextureCollection GetPlyMeshMtlTextures(PlyFile plyFile, string meshTextureName) {
            ResourceChecking.ThrowCheckPlyFileMeshTextureName(plyFile, meshTextureName);

            var cache = _cacheProvider.PlyTexturesCache ?? throw GohCacheException.CacheNotFount(nameof(_cacheProvider.PlyTexturesCache));

            var mtlTextures = new MtlTextureCollection();

            if (cache.TryGetValue(plyFile.Name, out var value)) {
                foreach (var texturePath in value) {
                    if (texturePath.Contains(meshTextureName)) {
                        var file = (MtlFile?)_resourceProvider.GetFile(texturePath);

                        if (file != null) { 
                            mtlTextures.Add(file.Data);
                        }
                    }
                }
            }

            return mtlTextures;
        }

        public void SetPlyFileFullPath(PlyFile plyFile) {
            if (!plyFile.IsLoaderInitialize) {
                plyFile.Loader = Root.Loader.FileLoader;
            }

            plyFile.RelativePathPoint = Root.GetFullPath();
        }

        public PlyFile GetNullPlyFile(PlyFile plyFile) {
            return new PlyFile(@"F:\SDK\Content\goh\entity\humanskin\[germans]\[ger_source]\ger_null.ply");
        }
    }
}
