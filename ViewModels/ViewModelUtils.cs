using System.Diagnostics;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;

namespace GohMdlExpert.ViewModels {
    public static class ViewModelUtils {
        public static void OpenInExplorer(GohResourceElement resourceElement) {
            IGohResourceLoader? loader = null;
            string? path = null;

            if (resourceElement is GohResourceFile file) {
                loader = file.Loader.ResourceLoader;
            } else if (resourceElement is GohResourceDirectory directory) {
                loader = directory.Loader.ResourceLoader;
            }

            if (loader is AggregateResourceLoader aggregateLoader) {
                loader = aggregateLoader.ResourceLoaders.FirstOrDefault(l => l is FileSystemResourceLoader);
                loader ??= aggregateLoader.ResourceLoaders.FirstOrDefault(l => l is PakResourceLoader);
            }

            if (loader is FileSystemResourceLoader fileSystemLoader) {
                path = fileSystemLoader.GetFileSystemPath(resourceElement.GetFullPath());
            } else if (loader is PakResourceLoader) {
                path = ((resourceElement as GohResourceFile)?.Loader as PakFileLoader)?.PakPath;
                path ??= ((resourceElement as GohResourceDirectory)?.Loader as PakDirectoryLoader)?.PakPath;
            }

            if (path != null) {
                Process.Start("explorer.exe", $"/select, {path}");
            }
        }
    }
}
