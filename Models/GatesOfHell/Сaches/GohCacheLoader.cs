using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GohMdlExpert.Models.GatesOfHell.Caches {
    public class GohCacheLoader : ICacheLoader {
        private static readonly string s_cacheDirectory = @"\Caches\";

        public GohCacheLoader() {
            string directoryPath = GetCacheDirectoryFullPath();

            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public Dictionary<string, T>? GetCache<T>(string key) {
            using var reader = GetReader(key);

            if (reader != null) {
                var d = JsonConvert.DeserializeObject<Dictionary<string, T>>(reader.ReadToEnd());

                return d;
            } else {
                return null;
            }
        }

        public void SetCache<T>(string key, Dictionary<string, T>? value) {
            DeleteCache(key);

            if (value != null) {
                using var writer = GetWriter(key);

                writer.Write(JsonConvert.SerializeObject(value));
            }
        }

        public void ClearCache() {
            string cacheDirectory = GetCacheDirectoryFullPath();

            if (Directory.Exists(cacheDirectory)) {
                foreach (var file in Directory.GetFiles(cacheDirectory)) {
                    File.Delete(file);
                } 
            }
        }

        private static StreamReader? GetReader(string key) {
            var stream = GetCacheStream(key);

            if (stream != null) {
                return new StreamReader(stream, leaveOpen: true);
            } else {
                return null;
            }
        }

        private static StreamWriter GetWriter(string key) {
            return new StreamWriter(GetCacheStream(key, true)!);
        }

        private static FileStream? GetCacheStream(string key, bool append = false) {
            string fileFullName = GetCacheFileFullName(key);

            if (!append && !File.Exists(fileFullName)) {
                return null;
            } else {
                return new FileStream(fileFullName, FileMode.OpenOrCreate);
            }
        }

        private static void DeleteCache(string key) {
            string fileFullName = GetCacheFileFullName(key);

            if (File.Exists(fileFullName)) {
                File.Delete(fileFullName);
            }
        }

        private static string GetCacheFileFullName(string key) {
            return Path.Join(GetCacheDirectoryFullPath(), key) + ".json";
        }

        private static string GetCacheDirectoryFullPath() {
            return Path.Join(Environment.CurrentDirectory, s_cacheDirectory);
        }
    }
}
