﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    internal class AggregateDirectoryLoader : IDirectoryLoader {
        private readonly List<IDirectoryLoader> _directoriesLoaders;

        public AggregateDirectoryLoader() {
            _directoriesLoaders = [];
        }

        public AggregateDirectoryLoader(IEnumerable<IDirectoryLoader> directoryLoaders) {
            _directoriesLoaders = [.. directoryLoaders.Reverse()];
        }

        public IFileLoader FileLoader => throw new NotImplementedException();

        public void AddDirectory(IDirectoryLoader directoryLoader) {
            _directoriesLoaders.Insert(0, directoryLoader);
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            var directories = new Dictionary<string, GohResourceDirectory>();

            foreach (var directoryLoader in _directoriesLoaders) {
                foreach (var directory in directoryLoader.GetDirectories(path)) {
                    if (!directories.TryAdd(directory.Name, directory)) {
                        var oldDirectory = directories[directory.Name];
                        directories[directory.Name] =
                            new GohResourceDirectory(directory.Name, directory.Path, directory.RelativePathPoint) {
                                Loader = new AggregateDirectoryLoader([directory.Loader, oldDirectory.Loader])
                            };
                    }   
                }
            }

            return directories.Values.OrderBy(d => d.Name);
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            var files = new Dictionary<string, GohResourceFile>();

            foreach (var directoryLoader in _directoriesLoaders) {
                foreach (var file in directoryLoader.GetFiles(path)) {
                    files.TryAdd(file.Name, file);
                }
            }

            return files.Values.OrderBy(d => d.Name);
        }
    }
}
