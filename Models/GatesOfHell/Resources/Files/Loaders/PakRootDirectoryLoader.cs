using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files.Loaders {
    public class PakRootDirectoryLoader : IDirectoryLoader {
        private readonly List<GohResourceDirectory> _resourceDirectories;

        public IFileLoader FileLoader => throw new InvalidOperationException("Pak base directory loader haven't files.");

        public PakRootDirectoryLoader() {
            _resourceDirectories = [];
        }

        public void AddPakDirectory(GohResourceDirectory resourceDirectory) {
            _resourceDirectories.Add(resourceDirectory);
        }

        public IEnumerable<GohResourceDirectory> GetDirectories(string path) {
            return [.. _resourceDirectories];
        }

        public IEnumerable<GohResourceFile> GetFiles(string path) {
            return [];
        }
    }
}
