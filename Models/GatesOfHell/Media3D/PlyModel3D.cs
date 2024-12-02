using System.Windows.Media.Media3D;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.Exceptions;

namespace GohMdlExpert.Models.GatesOfHell.Media3D {
    public class PlyModel3D {
        private readonly Dictionary<string, GeometryModel3D> _meshes;
        private readonly Dictionary<string, MtlTexture?> _meshesTextures;

        public Model3DGroup Model { get; }
        public PlyModel Ply { get; }
        public PlyFile? PlyFile { get; }
        public IEnumerable<string> MeshesNames => _meshes.Keys;

        public PlyModel3D(PlyModel plyModel, Dictionary<string, MtlTexture?>? meshesTextures = null) {
            Ply = plyModel;
            Model = ResourceConverts.PlyModelToModel3D(plyModel, meshesTextures);

            _meshesTextures = meshesTextures != null ? new(meshesTextures) : [];
            _meshes = LoadMeshes(plyModel, Model, PlyFile);
        }

        public PlyModel3D(PlyFile plyFile, Dictionary<string, MtlTexture?>? meshesTextures = null) : this(plyFile.Data, meshesTextures) {
            PlyFile = plyFile;
        }

        public PlyModel3D(PlyFile plyFile, PlyAggregateMtlFiles? mtlFiles) : this(plyFile.Data, mtlFiles?.GetFirstMeshesTextures()) {
            PlyFile = plyFile;
        }

        public static implicit operator Model3D?(PlyModel3D? ply) {
            return ply?.Model;
        }

        public void SetMeshTexture(string meshTextureName, MtlTexture? texture) {
            var mesh = GetMesh(meshTextureName);
            _meshesTextures[meshTextureName] = texture;
            mesh.Material = texture?.Diffuse.Data;
        }

        public MtlTexture? GetMeshTexture(string meshTextureName) {
            _ = GetMesh(meshTextureName);
            return _meshesTextures[meshTextureName];
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
                    if (meshTextures.Value == null) {
                        return true;
                    }
                }
            }

            return false;
        }

        private GeometryModel3D GetMesh(string meshTextureName) {
            if (!_meshes.TryGetValue(meshTextureName, out var mesh)) {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }

            return mesh;
        }

        private static Dictionary<string, GeometryModel3D> LoadMeshes(PlyModel plyModel, Model3DGroup model3DGroup, PlyFile? plyFile = null) {
            var meshesGeometries = new Dictionary<string, GeometryModel3D>();
            int meshIndex = 0;

            foreach (var modelMeshGeometry in model3DGroup.Children) {
                meshesGeometries.Add(plyModel.Meshes[meshIndex].TextureName, (GeometryModel3D)modelMeshGeometry);

                meshIndex++;
            }

            return meshesGeometries;
        }
    }
}
