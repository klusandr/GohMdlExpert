using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Extensions;
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
        public GohResourceDirectory? Humanskins { get; private set; }

        public GohHumanskinResource(GohResourceDirectory humanskinsDirectory, GohResourceProvider resourceProvider, GohCacheProvider cacheProvider) {
            Root = humanskinsDirectory;
            _resourceProvider = resourceProvider;
            _cacheProvider = cacheProvider;
            Load();
        }

        public void Load() {
            Source = GohResourceLoading.FilterResource(Root, (f) => f is PlyFile);
            Humanskins = GohResourceLoading.FilterResource(Root, (f) => f is MdlFile);
        }

        public void MdlMdoelInitialize(MdlModel mdlModel) {
            for (int i = 0; i < mdlModel.PlyModels.Length; i++) {
                var plyModel = mdlModel.PlyModels[i];
                bool isExists = plyModel.Exists();

                if (!isExists) {
                    mdlModel.PlyModels[i] = (PlyFile)(_resourceProvider.GetFile(plyModel.GetFullPath()) ?? throw GohResourceFileException.IsNotExists(plyModel));
                }

                if (mdlModel.PlyModelsLods.TryGetValue(plyModel, out var plyModelLods)) {
                    if (!isExists) {
                        mdlModel.PlyModelsLods.Remove(plyModel);
                        mdlModel.PlyModelsLods[mdlModel.PlyModels[i]] = plyModelLods;
                    }

                    for (int j = 0; j < plyModelLods.Length; j++) {
                        if (!plyModelLods[j].Exists()) {
                            plyModelLods[j] = (PlyFile)(_resourceProvider.GetFile(plyModelLods[j].GetFullPath()) ?? throw GohResourceFileException.IsNotExists(plyModelLods[j]));
                        }
                    }
                }
            }
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

            if (cache.TryGetValue(plyFile.Name, out var textureDirectories)) {
                foreach (var textureDirectory in textureDirectories) {
                    var directory = _resourceProvider.GetDirectory(textureDirectory);

                    if (directory != null) {
                        var file = directory.FindResourceElements<MtlFile>(meshTextureName, deepSearch: false, first: true).FirstOrDefault();

                        if (file != null) {
                            mtlTextures.Add(file.Data);
                        }
                    }
                }
            }

            return mtlTextures;
        }



        /// <summary>
        /// Возвращает коллекцию LOD файлов для указанного <see cref="PlyFile"/> файла.
        /// </summary>
        /// <param name="plyFile">Файл, для которого будут возвращены LOD файлы.</param>
        /// <returns>Коллекция <see cref="PlyFile"/> файлов, который являются LOD для указанного файла.</returns>
        public IEnumerable<PlyFile> GetPlyLodFiles(PlyFile plyFile) {
            var directory = _resourceProvider.GetDirectory(plyFile.GetDirectoryPath());

            var lodFiles = directory
                .FindResourceElements<PlyFile>(searchPattern: @$"{plyFile.Name[..^4]}_lod\d*\.", deepSearch: false);

            return lodFiles;
        }

        public PlyFile? GetNullPlyFile(PlyFile plyFile) {
            var pathComponents = PathUtils.GetPathComponents(plyFile.GetDirectoryPath()!);
            PlyFile? nullFile = null;

            for (int i = 0; i < pathComponents.Length; i++) {
                var directory = _resourceProvider.GetDirectory(PathUtils.GetPathFromComponents(pathComponents.SkipLast(i)));
                nullFile = directory!.FindResourceElements<PlyFile>(searchPattern: "null_*.ply", deepSearch: false, first: true).FirstOrDefault();
            }

            return nullFile;
        }
    }
}
