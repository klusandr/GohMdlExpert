using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders.Directories;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Loaders {
    public class AggregateResourceLoader : GohBaseResourceLoader {
        private readonly List<IGohResourceLoader> _resourceLoaders;
        private bool _isLoad = false;
        private GohResourceDirectory? _root;

        public override GohResourceDirectory? Root { 
            get {
                if (!_isLoad) {
                    LoadRootDictionary();
                }

                return _root;    
            }
            protected set => _root = value; 
        }

        public AggregateResourceLoader() {
            _resourceLoaders = [];
        }

        public AggregateResourceLoader(IEnumerable<IGohResourceLoader> resourceLoaders) {
            _resourceLoaders = [.. resourceLoaders];
        }

        public void AddResourceLoader(IGohResourceLoader resourceLoader) {
            _resourceLoaders.Insert(0, resourceLoader);
            _isLoad = false;
        }

        public override bool CheckRootPath(string path) {
            throw new InvalidOperationException("Aggregate resource loader not have a base path.");
        }

        public override void LoadData(string path) {
            throw new InvalidOperationException("Aggregate resource loader can't load data.");
        }

        private void LoadRootDictionary() {
            var loaderRoots = _resourceLoaders.Select((r) => r.Root ?? throw GohResourcesException.IsNotLoad());

            Root = new GohResourceDirectory(loaderRoots.First().GetFullPath()) {
                Loader = new AggregateDirectoryLoader(loaderRoots.Select(d => d.Loader))
            };

            _isLoad = true;
        }
    }
}
