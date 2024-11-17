using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemPath = System.IO.Path;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class GohResourceDirectory : GohResourceElement {
        public List<GohResourceElement>? Items { get; set; }

        public GohResourceDirectory(string name, string? path = null, string? relativePathPoint = null, string? location = null) 
            : base(name, path, relativePathPoint, location) {

            if (path == null) {
                name = name.Replace("/", "\\");
                int lastSeparateIndex = name.LastIndexOf('\\', name.Length - 2);

                if (lastSeparateIndex != -1) {
                    Path = name[..lastSeparateIndex];
                    Name = name[(lastSeparateIndex + 1)..];
                }
            }
        }

        public void LoadData() {
            if (Items != null) {
                Items.Clear();
            } else {
                Items = [];
            }

            foreach (var directoryNames in Directory.GetDirectories(GetFullPath())) {
                Items.Add(new GohResourceDirectory(directoryNames));
            }

            foreach (var filesNames in Directory.GetFiles(GetFullPath())) {
                Items.Add(GohResourceLoader.Instance.GetResourceFile(filesNames));
            }
        }

        public IEnumerable<GohResourceFile> GetFiles() {
            if (Items == null) { LoadData(); }

            return Items!.OfType<GohResourceFile>();   
        }

        public IEnumerable<GohResourceDirectory> GetDirectories() {
            if (Items == null) { LoadData(); }

            return Items!.OfType<GohResourceDirectory>();
        }
    }
}
