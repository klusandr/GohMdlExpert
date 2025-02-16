using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using Microsoft.Extensions.DependencyInjection;
using static GohMdlExpert.Models.GatesOfHell.Resources.Data.PlyModel;

namespace GohMdlExpert.Models.GatesOfHell.Caches {
    public static class GohCachesFilling {
        public static void FillPlyTexturesCache(GohResourceProvider resourceProvider, GohHumanskinResourceProvider humanskinProvider, ref float completionPercentage) {
            
            var cacheValues = new Dictionary<string, HashSet<string>>();

            foreach (var humanskinResource in humanskinProvider.HumanskinResources) {
                var mdlDirectories = humanskinResource.Root.GetDirectories().Where(dir => dir != humanskinResource.Source);

                float percentageSteep = 100 / mdlDirectories.Count();
                completionPercentage = 0;

                foreach (var directory in mdlDirectories) {
                    foreach (var mdlFile in directory.GetFiles().OfType<MdlFile>()) {
                        foreach (var plyFile in mdlFile.Data.PlyModel) {
                            humanskinResource.SetPlyFileFullPath(plyFile);
                            var loadPlyFile = (PlyFile)resourceProvider.ResourceLoader.GetFile(plyFile.GetFullPath().ToLower())!;

                            if (loadPlyFile != null) {
                                if (!cacheValues.TryGetValue(plyFile.Name, out var cacheValue)) {
                                    cacheValue = [];
                                    cacheValues[plyFile.Name] = cacheValue;
                                }

                                foreach (var mesh in loadPlyFile.Data.Meshes) {
                                    var mtlFile = directory.FindResourceElements<MtlFile>(searchPattern: mesh.TextureName, deepSearch: false, first: true).FirstOrDefault();

                                    if (mtlFile != null) {
                                        string insidePath = resourceProvider.GetInsidePath(mtlFile!.GetFullPath());
                                        cacheValue.Add(insidePath);
                                    }
                                }

                                loadPlyFile.UnloadData();
                            }
                        }
                    }

                    completionPercentage += percentageSteep;
                }

                completionPercentage = 100;
            }

            var cache = new Dictionary<string, string[]>();

            foreach (var cacheValue in cacheValues) {
                cache.Add(cacheValue.Key, [.. cacheValue.Value]);
            }

            GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>().PlyTexturesCache = cache;
        }

        public static void FillTexturesCache(GohResourceProvider resourceProvider, GohHumanskinResourceProvider humanskinProvider, ref float completionPercentage) {
            var cache = new Dictionary<string, string[]>();

            foreach (var humanskinResource in humanskinProvider.HumanskinResources) {
                var loadMtlFiles = humanskinResource.Root.FindResourceElements<MtlFile>();

                var mtlFiles = loadMtlFiles
                    .GroupBy(mf => new { mf.Name, mf.Data.Diffuse })
                    .Select(mfg => mfg.First())
                    .Select(mf => mf.GetFullPath().ToLower());
                cache.Add(humanskinResource.Name, mtlFiles.ToArray());

                completionPercentage = 100;
            }

            GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>().TexturesCache = cache;
        }
    }
}
