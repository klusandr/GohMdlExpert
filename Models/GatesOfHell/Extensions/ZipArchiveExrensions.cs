using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Extensions {
    public static class ZipArchiveExtensions {
        public static string GetArchiveDirectoryPath(this ZipArchive archive, string path) {
            string directoryPath = GetArchiveFilePath(archive, path);
            return directoryPath != "" ? directoryPath + '/' : directoryPath;
        }

        public static string GetArchiveFilePath(this ZipArchive archive, string path) {
            string[] pathComponents = GetPathComponents(path);
            string archivePath = "";

            foreach (var rootDirectory in archive.GetDirectoriesPathsByDeep(0)) {
                int inputIndex = Array.IndexOf(pathComponents, rootDirectory.Trim('/'));

                if (inputIndex != -1) {
                    archivePath = GetPathFromComponents(pathComponents.Skip(inputIndex));
                }
            }

            return archivePath;
        }

        public static IEnumerable<string> GetDirectoriesPathsByDeep(this ZipArchive archive, int deep) {
            var directoriesPaths = archive.Entries.Where(e => GetDeep(e.FullName) == deep && CheckDirectory(e.FullName)).Select(e => e.FullName);

            if (!directoriesPaths.Any()) {
                string? directoryPath = archive.GetDeepDirectoryPath(deep);

                if (directoryPath != null) {
                    directoriesPaths = [directoryPath];
                }
            }

            return directoriesPaths;
        }

        public static string? GetDeepDirectoryPath(this ZipArchive archive, int deep) {
            string? directoryPath = archive.Entries.FirstOrDefault(e => GetDeep(e.FullName) == deep + 1)?.FullName;

            return directoryPath != null ? GetPathFromComponents(GetPathComponents(directoryPath).Take(deep + 1)) : null;
        }

        public static bool CheckDirectory(string fullName) {
            return fullName.LastOrDefault() == '/';
        }

        public static int GetDeep(string fullName) {
            return fullName.Count((c) => c == '/') - (CheckDirectory(fullName) ? 1 : 0);
        }

        public static string[] GetPathComponents(string path) {
            return path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetPathFromComponents(IEnumerable<string> strings) {
            return $"{string.Join('/', strings)}";
        }

        public static string GetFullPath(string path, string archivePath, string elementPath) {
            string fullElementPath = elementPath.Trim('/');

            if (archivePath != "") {
                fullElementPath = fullElementPath.Replace(archivePath, null);
            }

            return $"{path}\\{fullElementPath}";
        }
    }
}
