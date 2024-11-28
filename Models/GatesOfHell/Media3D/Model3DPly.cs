using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using WpfMvvm.DependencyInjection;
using static GohMdlExpert.Models.GatesOfHell.Resources.PlyModel;

namespace GohMdlExpert.Models.GatesOfHell.Media3D {
    public class Model3DPly {
        private readonly Dictionary<string, GeometryModel3D> _meshes;
        private readonly Dictionary<string, MtlTextureCollection?>? _meshesTextures;

        public Model3DGroup Model { get; }
        public PlyModel Ply { get; }
        public PlyFile? PlyFile { get; }
        public IEnumerable<PlyAggregateMtlFile>? PlyMtlFiles { get; }
        public IEnumerable<string> MeshesNames => _meshes.Keys;

        public Model3DPly(PlyModel ply, IEnumerable<MtlTextureCollection?>? textures) {
            _meshesTextures = (textures != null) ? LoadTextures(ply, textures, PlyFile) : null;
            var mtlTextures = _meshesTextures?.Values.Select(tc => tc?.FirstOrDefault());

            Model = Model3DConverts.PlyModelToModel3D(ply, mtlTextures);
            Ply = ply;

            _meshes = LoadMeshes(ply, Model, PlyFile);
        }

        public Model3DPly(PlyModel ply, IEnumerable<MtlTexture> textures) : this(ply, textures.Select(t => new MtlTextureCollection([t]))) { }

        public Model3DPly(PlyFile plyFile, IEnumerable<PlyAggregateMtlFile>? textureFiles) : this(plyFile.Data, textureFiles?.Select(tf => tf.Data)) {
            PlyFile = plyFile;
            PlyMtlFiles = textureFiles;
        }

        public static implicit operator Model3D(Model3DPly? ply) {
            return ply?.Model;
        }

        public void SelectMeshTexture(string mashTextureName, int index) {
            var mesh = GetMesh(mashTextureName);
            var meshTextureCollection = GetMeshTextureCollection(mashTextureName);

            if (index < 0 || index >= meshTextureCollection.Count) {
                throw new PlyModelException(PlyFile, $"Model don't contain texture \"{mashTextureName}\" for mesh with index: {index}.");
            }

            #warning Исправить это....
            mesh.Material = GetMaterial(meshTextureCollection.ElementAt(index).Diffuse);
        }

        public bool CheckNoMaterial(string? meshTextureName = null) {
            if (_meshesTextures == null) {
                return true;
            }

            if (meshTextureName != null) {
                _ = GetMesh(meshTextureName);

                if (_meshesTextures[meshTextureName] == null) {
                    return true;
                }
            } else {
                foreach (var meshTextures in _meshesTextures) {
                    if (meshTextureName == null) {
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetMeshMaterialsCount(string meshTextureName) {
            return GetMeshTextureCollection(meshTextureName).Count;
        }

        private GeometryModel3D GetMesh(string mashTextureName) {
            if (_meshes.TryGetValue(mashTextureName, out var mesh)) {
                return mesh;
            } else {
                throw new PlyModelException(PlyFile, $"Ply model don't contain mesh with \"{mashTextureName}\" texture name.");
            }
        }

        private MtlTextureCollection GetMeshTextureCollection(string mashTextureName) {
            if (_meshesTextures == null || !_meshesTextures.TryGetValue(mashTextureName, out MtlTextureCollection? value)) {
                value = null;
            }

            return value ?? throw new PlyModelException(PlyFile, $"Ply model don't contain textures \"{mashTextureName}\" for mesh.");
        }

        private static Material GetMaterial(TextureFile mtlTexture) {
            return AppDependencyInjection.Instance.ServiceProvider.GetRequiredService<GohTextureProvider>().GetMaterial(mtlTexture);
        }

        private static Dictionary<string, GeometryModel3D> LoadMeshes(PlyModel plyModel, Model3DGroup model3DGroup, PlyFile? plyFile = null) {
            if (plyModel.Meshes.Length != model3DGroup.Children.Count) {
                throw new PlyModelException(plyFile, message: "Create Model3DPly error. Count PlyModel meshes don't match to count Model3DGeometry");
            }

            var meshesGeometries = new Dictionary<string, GeometryModel3D>();
            int meshIndex = 0;

            foreach (var modelMeshGeometry in model3DGroup.Children) {
                meshesGeometries.Add(plyModel.Meshes[meshIndex].TextureName, (GeometryModel3D)modelMeshGeometry);

                meshIndex++;
            }

            return meshesGeometries;
        }

        private static Dictionary<string, MtlTextureCollection?> LoadTextures(PlyModel plyModel, IEnumerable<MtlTextureCollection?> textures, PlyFile? plyFile = null) {
            if (plyModel.Meshes.Length != textures.Count()) {
                throw new PlyModelException(plyFile, $"Create {nameof(Model3DPly)} error. Count ply model meshes don't match to count set textures");
            }

            var meshesTextures = new Dictionary<string, MtlTextureCollection?>();
            int meshIndex = 0;

            foreach (var textureCollection in textures) {
                meshesTextures.Add(plyModel.Meshes[meshIndex].TextureName, textureCollection);

                meshIndex++;
            }

            return meshesTextures;
        }
    }
}
