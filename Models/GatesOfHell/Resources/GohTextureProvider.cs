﻿using System.IO;
using System.Linq;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources
{
    public class GohTextureProvider {
        private GohResourceDirectory? _textureDirectory;

        public GohResourceProvider GohResourceProvider { get; }

        public GohResourceDirectory TextureDirectory => _textureDirectory ?? throw TextureException.DirectoryNotSpecified();

        public bool IsResourceLoad => _textureDirectory != null;

        public event EventHandler? ResourceUpdated;

        public GohTextureProvider(GohResourceProvider gohResourceProvider) {
            GohResourceProvider = gohResourceProvider;
            gohResourceProvider.ResourceUpdated += ProviderResourceUpdated;
        }

        public void Update() {
            if (GohResourceProvider.IsResourceLoaded) {
                _textureDirectory = GohResourceProvider.GetLocationDirectory("texture");
                ResourceUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void TextureMaterialsInitialize(MtlTexture mtlTexture) {
            if (!mtlTexture.IsMaterialsInitialize) {
                mtlTexture.Diffuse = GetMaterialFile(mtlTexture.DiffusePath) ?? throw GohResourceFileException.IsNotExists(mtlTexture.DiffusePath);

                if (mtlTexture.BumpPath != null) {
                    mtlTexture.Bump = GetMaterialFile(mtlTexture.BumpPath) ?? throw GohResourceFileException.IsNotExists(mtlTexture.BumpPath);
                }

                if (mtlTexture.SpecularPath != null) {
                    mtlTexture.Specular = GetMaterialFile(mtlTexture.SpecularPath) ?? throw GohResourceFileException.IsNotExists(mtlTexture.SpecularPath);
                }
            }
        }

        public void SetTexturesMaterialsFullPath(IEnumerable<MtlTexture> mtlTextures) {
            foreach (var mtlTexture in mtlTextures) {
                TextureMaterialsInitialize(mtlTexture);
            }
        }

        private MaterialFile? GetMaterialFile(string materialPath) {
            return TextureDirectory.AlongPath(Path.GetDirectoryName(materialPath)!)?.GetFile(Path.GetFileName(materialPath) + ".dds") as MaterialFile;
        }

        private void ProviderResourceUpdated(object? sender, EventArgs e) {
            Update();
        }
    }
}
