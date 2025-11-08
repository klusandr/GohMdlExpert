using System.Diagnostics;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;

namespace GohMdlExpert.ViewModels {
    public static class ViewModelUtils {
        public static void OpenInExplorer(GohResourceElement resourceElement) {
            string? path = ((resourceElement as GohResourceFile)?.Loader as PakFileLoader)?.PakPath;

            path ??= ((resourceElement as GohResourceDirectory)?.Loader as PakDirectoryLoader)?.PakPath;
            path ??= resourceElement.GetFullPath();

            Process.Start("explorer.exe", $"/select, {path}");
        }
    }
}
