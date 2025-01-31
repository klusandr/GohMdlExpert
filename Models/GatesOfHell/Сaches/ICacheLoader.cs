
namespace GohMdlExpert.Models.GatesOfHell.Caches {
    public interface ICacheLoader {
        Dictionary<string, T>? GetCache<T>(string key);
        void SetCache<T>(string key, Dictionary<string, T>? value);
        void ClearCache();
    }
}