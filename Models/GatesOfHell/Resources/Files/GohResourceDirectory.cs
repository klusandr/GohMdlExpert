using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using Microsoft.Extensions.DependencyInjection;

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
            set {
                _items = value;
            }
        }

        public IDirectoryLoader Loader {
            get => _loader ?? GohServicesProvider.Instance.GetRequiredService<IDirectoryLoader>();
            set {
                if (_loader != null) throw new InvalidOperationException($"Cannot reinitialize property {nameof(Loader)}.");
                _loader = value;
            }
        }

        public GohResourceDirectory(string name, string? path = null, string? relativePathPoint = null)
            : base(name, path, relativePathPoint) {

            if (path == null) {
                name = name.Replace("/", "\\");
                int lastSeparateIndex = name.LastIndexOf('\\', name.Length - 2);

                if (lastSeparateIndex != -1) {
                    Path = name[..lastSeparateIndex];
                    Name = name[(lastSeparateIndex + 1)..];
                }
            }
        }

        public GohResourceDirectory(GohResourceElement resourceElement) : this(resourceElement.GetDirectoryPath() ?? throw GohResourcesException.PathIsNull(resourceElement)) { }

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
            return AlongPath(path.Split('\\', StringSplitOptions.RemoveEmptyEntries));
        }

        public GohResourceDirectory? AlongPath(IEnumerable<string> pathDirectoryNames) {
            GohResourceDirectory? currentDirectory = null;

            if (pathDirectoryNames.Any()) {
                currentDirectory = GetDirectory(pathDirectoryNames.First());

                if (currentDirectory != null && pathDirectoryNames.Count() > 1) {
                    currentDirectory = currentDirectory.AlongPath(pathDirectoryNames.Skip(1));
                }
            }

            return currentDirectory;
        }

        public GohResourceDirectory? GetDirectory(string name) {
            return Items.FirstOrDefault(d => d.Name == name) as GohResourceDirectory;
        }

        public GohResourceFile? GetFile(string name) {
            return Items.FirstOrDefault(d => d.Name == name) as GohResourceFile;
        }

        public IEnumerable<GohResourceElement> FindResourceElements(string? resourceName = null, string? searchPattern = null, bool deepSearch = true, bool first = false) {
            return FindResourceElements(GetPredicate(resourceName, searchPattern, null), deepSearch, first);
        }

        public IEnumerable<T> FindResourceElements<T>(string? resourceName = null, string? searchPattern = null, bool deepSearch = true, bool first = false) where T : GohResourceElement {
            return FindResourceElements(GetPredicate(resourceName, searchPattern, typeof(T)), deepSearch, first).Select(resource => (T)resource);
        }

        public IEnumerable<GohResourceElement> FindResourceElements(Func<GohResourceElement, bool> predicate, bool deepSearch = true, bool first = false) {
            return FindResourceElements(predicate, deepSearch, first, null);
        }

        private List<GohResourceElement> FindResourceElements(Func<GohResourceElement, bool> predicate, bool deepSearch = true, bool first = false, List<GohResourceElement>? items = null) {
            items ??= [];

            foreach (var item in Items) {
                if (predicate(item)) {
                    items.Add(item);

                    if (first) { return items; }
                }
            }

            if (deepSearch) {
                foreach (var item in GetDirectories()) {
                    item.FindResourceElements(predicate, deepSearch, first, items);
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
