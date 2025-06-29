using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceVirtualDirectory : GohResourceDirectory {
        public GohResourceVirtualDirectory(string name, string? path = null, string? relativePathPoint = null) : base(name, path, relativePathPoint) {
            Items = [];
        }

        public GohResourceVirtualDirectory(GohResourceDirectory resourceDirectory, bool loadFiles = false, bool loadDirectories = false, bool deepLoad = false) : this(resourceDirectory.Name, resourceDirectory.Path, resourceDirectory.RelativePathPoint) {
            Loader = resourceDirectory.Loader;

            if (loadFiles) {
                LoadFiles(resourceDirectory);
            }

            if (loadDirectories) {
                LoadDirectories(resourceDirectory, loadFiles, deepLoad);
            }
        }

        public static GohResourceVirtualDirectory GetDeepClone(GohResourceDirectory resourceDirectory) {
            return new GohResourceVirtualDirectory(resourceDirectory, true, true, true);
        }

        public override void LoadData() { }

        public GohResourceVirtualDirectory LoadFiles(GohResourceDirectory resourceDirectory) {
            Items.AddRange(resourceDirectory.GetFiles());

            return this;
        }

        public GohResourceVirtualDirectory LoadDirectories(GohResourceDirectory resourceDirectory, bool loadFiles = false, bool deepLoad = false) {
            foreach (var directory in resourceDirectory.GetDirectories()) {
                Items.Add(new GohResourceVirtualDirectory(directory, loadFiles, deepLoad, deepLoad));
            }

            return this;
        }

        public override void ClearData() {
            Items.Clear();
        }

        public GohResourceVirtualDirectory AlongPathOrCreate(string path, Func<string, GohResourceVirtualDirectory>? dirCreate = null) {
            return AlongPathOrCreate(path.Split('\\', StringSplitOptions.RemoveEmptyEntries), dirCreate);
        }

        public GohResourceVirtualDirectory AlongPathOrCreate(IEnumerable<string> pathDirectoryNames, Func<string, GohResourceVirtualDirectory>? dirCreate = null) {
            if (pathDirectoryNames.Any()) {
                string directoryName = pathDirectoryNames.First();

                var currentDirectory = (GohResourceVirtualDirectory?)GetDirectory(directoryName);

                if (currentDirectory == null) {
                    currentDirectory = dirCreate?.Invoke(GetFullPath()) ?? new GohResourceVirtualDirectory(directoryName, GetFullPath()) {};
                    Items.Add(currentDirectory);
                }

                if (pathDirectoryNames.Count() > 1) {
                    currentDirectory = currentDirectory.AlongPathOrCreate(pathDirectoryNames.Skip(1));
                }

                return currentDirectory;
            }

            return this;
        }
    }
}
