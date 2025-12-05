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
        public static void FillPlyTexturesCache(GohResourceProvider resourceProvider, IGohHumanskinResource humanskinResource, CancellationToken? cancellationToken = null, Action<string, float>? currentPlyFileHandler = null) {
            var cacheValues = new Dictionary<string, HashSet<string>>();

            var mdlDirectories = humanskinResource.Root.FindResourceElements(
                (e) => {
                    if (e is GohResourceDirectory directory) {
                        return directory.Items.Any(f => f is MdlFile);
                    }

                    return false;
                }
            ).Cast<GohResourceDirectory>();

            float percentageSteep = 100f / mdlDirectories.Count();
            float completionPercentage = 0;

            foreach (var directory in mdlDirectories) {
                foreach (var mdlFile in directory.GetFiles().OfType<MdlFile>()) {
                    foreach (var plyFile in mdlFile.Data.PlyModel) {
                        cancellationToken?.ThrowIfCancellationRequested();
                        var loadPlyFile = (PlyFile)resourceProvider.GetFile(plyFile.GetFullPath().ToLower())!;

                        currentPlyFileHandler?.Invoke(loadPlyFile.GetFullPath(), completionPercentage);

                        if (loadPlyFile != null) {
                            if (!cacheValues.TryGetValue(plyFile.Name, out var cacheValue)) {
                                cacheValue = [];
                                cacheValues[plyFile.Name] = cacheValue;
                            }

                            cacheValue.Add(mdlFile.GetDirectoryPath()!);
                        }
                    }
                }

                completionPercentage += percentageSteep;
            }

            completionPercentage = 100;

            var cache = new Dictionary<string, string[]>();

            foreach (var cacheValue in cacheValues) {
                cache.Add(cacheValue.Key, [.. cacheValue.Value]);
            }

            GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>().PlyTexturesCache = cache;
        }

        public static void FillTexturesCache(GohTextureProvider textureProvider, IGohHumanskinResource humanskinResource, CancellationToken? cancellationToken = null, Action<string, float>? currentFileHandler = null) {
            var cache = new Dictionary<string, string[]>();

            var loadMtlFiles = humanskinResource.Root.FindResourceElements<MtlFile>();
            var errorMtlFiles = new List<MtlFile>();
            var steap = 100f / loadMtlFiles.Count();

            float completionPercentage = 0;

            foreach (var mtlFile in loadMtlFiles) {
                cancellationToken?.ThrowIfCancellationRequested();

                try {
                    textureProvider.TextureMaterialsInitialize(mtlFile.Data);
                } catch (Exception) {
                    errorMtlFiles.Add(mtlFile);
                }

                completionPercentage += steap;
                currentFileHandler?.Invoke(mtlFile.GetFullPath(), completionPercentage);
            }

            loadMtlFiles = loadMtlFiles.Except(errorMtlFiles);

            var mtlFiles = loadMtlFiles
                .GroupBy(mf => new { mf.Name, mf.Data.Diffuse })
                .Select(mfg => mfg.First())
                .Select(mf => mf.GetFullPath().ToLower());
            cache.Add(humanskinResource.Root.Name, mtlFiles.ToArray());

            GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>().TexturesCache = cache;
        }
    }
}
