using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohGameDirectory {
        private const string GAME_EXE_PATH = @"\binaries\x64\call_to_arms.exe";
        private const string RESOURCE_DIR_NAME = @"resource";

        public string? Path { get; private set; }
        public string? ResourcePath { get; private set; }
        public Version? Version { get; private set; }

        public event EventHandler? Updated;

        public void Open(string path) {
            var gameFile = SystemPath.Join(path, GAME_EXE_PATH);
            var resourceDirectory = SystemPath.Join(path, RESOURCE_DIR_NAME);

            if (!File.Exists(gameFile) || !Directory.Exists(resourceDirectory)) {
                throw GohResourcesException.IsNotGohGameDirectory(path);
            }

            var gameVersion = FileVersionInfo.GetVersionInfo(gameFile);

            Path = path;
            Version = new Version(gameVersion.FileVersion!); 
            ResourcePath = resourceDirectory;

            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
