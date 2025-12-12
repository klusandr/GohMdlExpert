using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceVirtualDirectory : GohResourceDirectory {
        private readonly GohResourceDirectory? _resourceDirectory;
        private readonly bool _loadFiles;
        private readonly bool _loadDirectories;
        private readonly bool _deepLoad;
        private readonly Func<GohResourceFile, bool>? _filter;
        private readonly bool _skipEmptyDirectories;

        public GohResourceVirtualDirectory(string name, string? path = null, string? relativePathPoint = null) : base(name, path, relativePathPoint) {
            Items = [];
        }

        public GohResourceVirtualDirectory(GohResourceDirectory resourceDirectory, bool loadFiles = false, bool loadDirectories = false, bool deepLoad = false, Func<GohResourceFile, bool>? filter = null, bool skipEmptyDirectories = false) : this(resourceDirectory.Name, resourceDirectory.Path, resourceDirectory.RelativePathPoint) {
            _resourceDirectory = resourceDirectory;
            _loadFiles = loadFiles;
            _loadDirectories = loadDirectories;
            _deepLoad = deepLoad;
            _filter = filter;
            _skipEmptyDirectories = skipEmptyDirectories;

            Loader = resourceDirectory.Loader;

            resourceDirectory.Update += ResourceDirectoryUpdateHandler;

            LoadData();
        }

        private void ResourceDirectoryUpdateHandler(object? sender, EventArgs e) {
            base.UpdateData();
        }

        public static GohResourceVirtualDirectory GetDeepClone(GohResourceDirectory resourceDirectory, Func<GohResourceFile, bool>? filter = null, bool skipEmptyDirectories = false) {
            return new GohResourceVirtualDirectory(resourceDirectory, true, true, true, filter, skipEmptyDirectories);
        }

        public override void LoadData() {
            if (_resourceDirectory != null) {
                if (_loadFiles) {
                    LoadFiles(_resourceDirectory, _filter);
                }

                if (_loadDirectories) {
                    LoadDirectories(_resourceDirectory, _loadFiles, _deepLoad, _filter);
                }
            }
        }

        public override void UpdateData() {
            if (_resourceDirectory != null) {
                _resourceDirectory.UpdateData();
                base.UpdateData();
            }
        }

        public GohResourceVirtualDirectory LoadFiles(GohResourceDirectory resourceDirectory, Func<GohResourceFile, bool>? filter = null) {
            var files = resourceDirectory.GetFiles();

            if (filter != null) {
                files = files.Where(filter);
            }

            Items.AddRange(files);

            return this;
        }

        public GohResourceVirtualDirectory LoadDirectories(GohResourceDirectory resourceDirectory, bool loadFiles = false, bool deepLoad = false, Func<GohResourceFile, bool>? filter = null) {
            foreach (var directory in resourceDirectory.GetDirectories()) {
                var virualDirectory = new GohResourceVirtualDirectory(directory, loadFiles, deepLoad, deepLoad, filter);

                if (!_skipEmptyDirectories || virualDirectory.Items.Count != 0) {
                    Items.Add(virualDirectory);
                }
            }

            return this;
        }

        public override void ClearData() {
            Items.Clear();
        }

        public GohResourceVirtualDirectory AlongPathOrCreate(string path, Func<string, string, GohResourceVirtualDirectory>? dirCreate = null) {
            return AlongPathOrCreate(path.Split('\\', StringSplitOptions.RemoveEmptyEntries), dirCreate);
        }

        public GohResourceVirtualDirectory AlongPathOrCreate(IEnumerable<string> pathDirectoryNames, Func<string, string, GohResourceVirtualDirectory>? dirCreate = null) {
            if (pathDirectoryNames.Any()) {
                string directoryName = pathDirectoryNames.First();

                var currentDirectory = (GohResourceVirtualDirectory?)GetDirectory(directoryName);

                if (currentDirectory == null) {
                    currentDirectory = dirCreate?.Invoke(directoryName, GetFullPath()) ?? new GohResourceVirtualDirectory(directoryName, GetFullPath()) {};
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
