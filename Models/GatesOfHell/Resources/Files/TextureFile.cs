using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static GohMdlExpert.Models.GatesOfHell.Resources.PlyModel;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Files {
    public class TextureFile(string name, string? path = null, string? relativePathPoint = null, string? location = null) 
        : GohResourceFile(name, path, relativePathPoint, location) {

        public new DiffuseMaterial Data {
            get {
                var diffuseMaterial = new DiffuseMaterial(
                    new ImageBrush(new BitmapImage(new Uri(GetFullPath()))) {
                        ViewportUnits = BrushMappingMode.Absolute,
                    }
                );

                diffuseMaterial.Freeze();
                diffuseMaterial.Brush.Freeze();

                return diffuseMaterial;
            }
        }

        public override void LoadData() { }
    }
}
