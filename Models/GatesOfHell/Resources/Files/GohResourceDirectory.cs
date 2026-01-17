using System.Diagnostics;
using System.Text.RegularExpressions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    [DebuggerDisplay("Path = {ToString()}")]
    public class GohResourceDirectory : GohResourceElement {
        private List<GohResourceElement>? _items;

        private IDirectoryLoader? _loader;

        public List<GohResourceElement> Items {
            get {
                if (_items == null) {
                    LoadData();
                }

                return _items!;
            }
            protected set {
                _items = value;
            }
        }

        public IDirectoryLoader Loader {
            get => _loader ?? throw GohResourceDirectoryException.LoaderIsNull(this);
            set {
                _loader = value;
            }
        }

        public event EventHandler? Update;

        public GohResourceDirectory(string name, string? path = null, string? relativePathPoint = null)
            : base(name, path, relativePathPoint) {

            if (path == null) {
                name = name.Replace('/', GohResourceLoading.DIRECTORY_SEPARATE);
                int lastSeparateIndex = name.LastIndexOf(GohResourceLoading.DIRECTORY_SEPARATE, name.Length - 2);

                if (lastSeparateIndex != -1) {
                    Path = name[..lastSeparateIndex];
                    Name = name[(lastSeparateIndex + 1)..].Trim(GohResourceLoading.DIRECTORY_SEPARATE);
                }
            }
        }

        public GohResourceDirectory(GohResourceElement resourceElement) : this(resourceElement.GetDirectoryPath() ?? throw GohResourceLoadException.PathIsNull(resourceElement)) { }

        public bool Exists() {
            return Loader.Exists(GetFullPath());
        }

        public virtual void LoadData() {
            if (_items != null) {
                Items.Clear();
            } else {
                _items = [];
            }

            foreach (var directory in Loader.GetDirectories(GetFullPath())) {
                _items.Add(directory);
            }

            foreach (var file in Loader.GetFiles(GetFullPath())) {
                _items.Add(file);
            }
        }

        public virtual void UpdateData() {
            ClearData();
            LoadData();
            Update?.Invoke(this, EventArgs.Empty);
        }

        public virtual void ClearData() {
            _items = null;
        }

        public IEnumerable<GohResourceFile> GetFiles() {
            if (Items == null) { LoadData(); }

            return Items!.OfType<GohResourceFile>();
        }

        public IEnumerable<GohResourceDirectory> GetDirectories() {
            if (Items == null) { LoadData(); }

            return Items!.OfType<GohResourceDirectory>();
        }

        public GohResourceDirectory? AlongPath(string path) {
            return AlongPath(PathUtils.GetPathComponents(path));
        }

        public GohResourceDirectory? AlongPath(IEnumerable<string> pathDirectoryNames) {
            GohResourceDirectory? currentDirectory = this;

            if (pathDirectoryNames.Any()) {
                currentDirectory = GetDirectory(pathDirectoryNames.First());

                if (currentDirectory != null && pathDirectoryNames.Count() > 1) {
                    currentDirectory = currentDirectory.AlongPath(pathDirectoryNames.Skip(1));
                }
            }

            return currentDirectory;
        }

        public GohResourceDirectory? GetDirectory(string name) {
            return Items.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)) as GohResourceDirectory;
        }

        public GohResourceFile? GetFile(string name) {
            return Items.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) as GohResourceFile;
        }

        public IEnumerable<GohResourceElement> FindResourceElements(string? resourceName = null, string? searchPattern = null, bool deepSearch = true, bool first = false) {
            return FindResourceElements(GetPredicate(resourceName, searchPattern, null), deepSearch, first);
        }

        public IEnumerable<T> FindResourceElements<T>(string? resourceName = null, string? searchPattern = null, bool deepSearch = true, bool first = false) where T : GohResourceElement {
            return FindResourceElements(GetPredicate(resourceName, searchPattern, typeof(T)), deepSearch, first).Select(resource => (T)resource);
        }

        public IEnumerable<GohResourceElement> FindResourceElements(Func<GohResourceElement, bool> predicate, bool deepSearch = true, bool first = false) {
            if (!deepSearch) {
                if (first) {
                    return FindResourceElementLocal(predicate);
                } else {
                    return FindResourceElementsLocal(predicate);
                }
            } else {
                if (first) {
                    return FindResourceElement(predicate);
                } else {
                    return FindResourceElements(predicate, null);
                }
            }
        }

        private List<GohResourceElement> FindResourceElementLocal(Func<GohResourceElement, bool> predicate) {
            foreach (var item in Items) {
                if (predicate(item)) {
                    return [item];
                }
            }

            return [];
        }

        private List<GohResourceElement> FindResourceElementsLocal(Func<GohResourceElement, bool> predicate) {
            var items = new List<GohResourceElement>();

            foreach (var item in Items) {
                if (predicate(item)) {
                    items.Add(item);
                }
            }

            return items;
        }

        private List<GohResourceElement> FindResourceElement(Func<GohResourceElement, bool> predicate) {
            foreach (var item in Items) {
                if (predicate(item)) {
                    return [item];
                }
            }

            foreach (var directory in GetDirectories()) {
                directory.FindResourceElement(predicate);
            }

            return [];
        }

        private List<GohResourceElement> FindResourceElements(Func<GohResourceElement, bool> predicate, List<GohResourceElement>? items = null) {
            items ??= [];

            foreach (var item in Items) {
                if (predicate(item)) {
                    items.Add(item);
                }

                if (item is GohResourceDirectory directory) {
                    directory.FindResourceElements(predicate, items);
                }
            }

            return items;
        }

        private Func<GohResourceElement, bool> GetPredicate(string? resourceName = null, string? searchPattern = null, Type? resourceType = null) {
            var searchRegex = (searchPattern != null) ? new Regex(searchPattern) : null;

            bool predicate(GohResourceElement item) {
                if (resourceName == null || item.Name == resourceName) {
                    if (searchRegex == null || searchRegex.IsMatch(item.Name)) {
                        if (resourceType == null || item.GetType() == resourceType) {
                            return true;
                        }
                    }
                }

                return false;
            }


            return predicate;
        }
    }
}
