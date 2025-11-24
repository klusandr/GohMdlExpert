using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Loaders;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Mods {
    public class OutputModResource {
        private readonly ModResourceLoader _resourceLoader;
        private readonly GohResourceVirtualDirectory _root;

        public string Name { get; }
        public string Path { get; }
        public GohResourceDirectory Root => _root;
        public IFileLoader FileLoader => _resourceLoader.FileLoader;

        public OutputModResource(string name, string path) {
            Name = name;
            Path = path;
            _resourceLoader = new ModResourceLoader(path);

            if (Directory.Exists(path)) {
                _root = GohResourceVirtualDirectory.GetDeepClone(_resourceLoader.Root);
            } else {
                _root = new GohResourceVirtualDirectory("");
            }
        }

        public OutputModResource(string path) : this(SystemPath.GetFileName(path), path) { }

        public void CreateModDirectories() {
            Directory.CreateDirectory(Path);

            CreateSubDirectories(Root);
        }

        public void AddDirectory(string path) {
            _root.AlongPathOrCreate(path);
        }

        public void AddFile(GohResourceFile resourceFile) {
            var directory = _root.AlongPathOrCreate(resourceFile.GetDirectoryPath()!);

            directory.Items.Add(resourceFile);
        }

        private void CreateSubDirectories(GohResourceDirectory directory) {
            foreach (var subDirectory in directory.GetDirectories()) {
                Directory.CreateDirectory(_resourceLoader.GetFileSystemPath(subDirectory.GetFullPath() ?? ""));

                if (subDirectory.GetDirectories().Any()) {
                    CreateSubDirectories(subDirectory);
                }
            }
        }
    }
}
