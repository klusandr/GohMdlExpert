using System.IO;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class OutputModResource : ModResource {
        private readonly GohResourceVirtualDirectory _root;

        public GohResourceVirtualDirectory Root => _root;

        public OutputModResource(string path) : base(path) {
            if (Directory.Exists(path)) {
                _root = GohResourceVirtualDirectory.GetDeepClone(ResourceLoader.Root);
            } else {
                _root = new GohResourceVirtualDirectory("");
            }
        }

        public void CreateModDirectories() {
            Directory.CreateDirectory(Path);

            CreateSubDirectories(Root);
        }


        public GohResourceDirectory AddDirectory(string fullPath) {
            var directory = _root.AlongPathOrCreate(fullPath);

            if (!directory.Exists()) {
                Directory.CreateDirectory(ResourceLoader.GetFileSystemPath(directory.GetFullPath()));
            }

            return directory;
        }

        internal void RemoveDirectory(string fullPath, bool recursive = false) {
            var directory = _root.AlongPathOrCreate(fullPath);

            if (directory.Exists()) {
                Directory.Delete(ResourceLoader.GetFileSystemPath(directory.GetFullPath()), recursive);
            }
        }


        public void AddFile(GohResourceFile resourceFile, GohResourceDirectory resourceDirectory) {
            var directory = AddDirectory(resourceDirectory.GetFullPath());

            resourceFile.Loader = ResourceLoader.FileLoader;
            resourceFile.RelativePathPoint = null;
            resourceFile.Path = directory.GetFullPath();

            directory.Items.Add(resourceFile);
            resourceFile.SaveData();
        }

        private void CreateSubDirectories(GohResourceDirectory directory) {
            foreach (var subDirectory in directory.GetDirectories()) {
                Directory.CreateDirectory(ResourceLoader.GetFileSystemPath(subDirectory.GetFullPath() ?? ""));

                if (subDirectory.GetDirectories().Any()) {
                    CreateSubDirectories(subDirectory);
                }
            }
        }
    }
}
