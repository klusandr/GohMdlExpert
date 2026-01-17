using System.Runtime.CompilerServices;
using GohMdlExpert.Models.GatesOfHell.Caches;

namespace GohMdlExpert.Models.GatesOfHell.Сaches {
    public class GohCacheProvider {
        private readonly Dictionary<string, object?> _cache;

        public ICacheLoader CacheLoader { get; }

        public Dictionary<string, string[]>? PlyTexturesCache { get => GetCache<string[]>(); set => SetCache(value); }
        public Dictionary<string, string[]>? TexturesCache { get => GetCache<string[]>(); set => SetCache(value); }

        public GohCacheProvider(ICacheLoader cacheLoader) {
            _cache = [];
            CacheLoader = cacheLoader;
        }

        public void ClearCache() {
            _cache.Clear();
        }

        private Dictionary<string, T>? GetCache<T>([CallerMemberName] string? key = null) {
            key = key!.Replace("Cache", null);

            if (!_cache.TryGetValue(key, out var cache)) {
                cache = CacheLoader.GetCache<T>(key!);
                _cache.Add(key!, cache);
            }

            return (Dictionary<string, T>?)cache;
        }

        private void SetCache<T>(Dictionary<string, T>? value, [CallerMemberName] string? key = null) {
            key = key!.Replace("Cache", null);
            _cache[key] = value;
            CacheLoader.SetCache<T>(key, value);
        }
    }
}
