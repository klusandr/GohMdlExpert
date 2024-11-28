using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohTextureProvider {
        public GohResourceProvider GohResourceProvider { get; }

        public GohTextureProvider(GohResourceProvider gohResourceProvider) {
            GohResourceProvider = gohResourceProvider;
        }

        public Material? GetMaterial(TextureFile textureFile) {
            string? fullPath = GetTextureFullPath(textureFile);
            DiffuseMaterial? diffuseMaterial = null;

            if (fullPath != null) {
                diffuseMaterial = new DiffuseMaterial(
                    new ImageBrush(new BitmapImage(new Uri(fullPath))) {
                        ViewportUnits = BrushMappingMode.Absolute,
                    }
                );

                diffuseMaterial.Freeze();
                diffuseMaterial.Brush.Freeze();
            }

            return diffuseMaterial;
        }

        private string? GetTextureFullPath(TextureFile textureFile) {
            string fullPath = Path.Combine(GohResourceProvider.GetLocationDirectory("texture").GetFullPath(), textureFile.GetFullPath());

            return fullPath.Replace("$", "");
        }
    }
}
