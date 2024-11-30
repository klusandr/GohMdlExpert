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
using GohMdlExpert.Models.GatesOfHell.Media3D;
using System.Data;
using GohMdlExpert.Models.GatesOfHell.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class GohTextureProvider {
        public GohResourceProvider GohResourceProvider { get; }

        public GohResourceDirectory? TextureDirectory { get; protected set; }

        public event EventHandler? ResourceUpdated;

        public GohTextureProvider(GohResourceProvider gohResourceProvider) {
            GohResourceProvider = gohResourceProvider;
            gohResourceProvider.ResourceUpdated += ProviderResourceUpdated;
        }

        public void Update() {
            if (GohResourceProvider.IsResourceLoaded) { 
                TextureDirectory = GohResourceProvider.GetLocationDirectory("texture");
                ResourceUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetTextureMaterialsFullPath(MtlTexture mtlTexture) {
            GohResourceFile?[] materialFiles = [mtlTexture.Diffuse, mtlTexture.Bump, mtlTexture.Specular];

            foreach (var materialFile in materialFiles) {
                if (materialFile != null) {
                    SetMaterialFullPath((MaterialFile)materialFile);
                }
            }
        }

        private MaterialFile SetMaterialFullPath(MaterialFile materialFile) {
            if (TextureDirectory == null) {
                throw new GohResourcesException("Error load textures. Texture directory is not specified.");
            }

            if (materialFile.IsRelativePath && materialFile.RelativePathPoint == null) {
                materialFile.RelativePathPoint = TextureDirectory.GetFullPath();
            }

            return materialFile;
        }

        private void ProviderResourceUpdated(object? sender, EventArgs e) {
            Update();
        }
    }
}
