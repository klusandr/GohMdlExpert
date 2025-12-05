using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public static class PathUtils {
        public static string[] GetPathComponents(string path) {
            return path.Split(GohResourceLoading.DIRECTORY_SEPARATE, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetPathFromComponents(IEnumerable<string> strings) {
            return $"{string.Join(GohResourceLoading.DIRECTORY_SEPARATE, strings)}";
        }

        public static string GetPathFromFirstElments(string path, int elementCount) {
            return GetPathFromComponents(GetPathComponents(path).Take(elementCount));
        }
    }
}
