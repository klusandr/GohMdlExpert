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
            var directoriesPaths = new List<string>();
            var entriesPaths = archive.Entries.Where(e => GetDeep(e.FullName) > deep).Select(e => e.FullName);

            foreach (var directory in entriesPaths) {
                if (!directoriesPaths.Any(directory.Contains)) {
                    directoriesPaths.Add(GetPathFromComponents(GetPathComponents(directory).Take(deep + 1)) + '/');
                }
            }

            return directoriesPaths;
        }

        public static string? GetDeepDirectoryPath(this ZipArchive archive, int deep) {
            string? directoryPath = archive.Entries.FirstOrDefault(e => GetDeep(e.FullName) == deep + 1)?.FullName;

            return directoryPath != null ? GetPathFromComponents(GetPathComponents(directoryPath).Take(deep + 1)) : null;
        }

        public static bool CheckDirectory(string fullName) {
            return fullName.LastOrDefault() is ('/' or '\\');
        }

        public static int GetDeep(string fullName) {
            return fullName.Count((c) => c is ('/' or '\\')) - (CheckDirectory(fullName) ? 1 : 0);
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

            return @$"{path}\{fullElementPath}";
        }

        public static string GetPathFromFirstElments(string path, int elementCount) {
            return GetPathFromComponents(GetPathComponents(path).Take(elementCount));
        }

        public static string GetZipPath(string path) {
            return path.Replace('\\', '/');
        }
    }
}
