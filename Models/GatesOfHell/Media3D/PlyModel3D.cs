using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;

namespace GohMdlExpert.Models.GatesOfHell.Media3D
{
    public class PlyModel3D {
        private class MeshData {
            private readonly string _textureName;
            private readonly GeometryModel3D _geometry;
            private readonly ScaleTransform3D _scaleTransform;
            private MtlTexture? _texture;
            private bool _isVisible;

            public string TextureName => _textureName;
            public GeometryModel3D Mesh => _geometry;

            public MtlTexture? Texture {
                get => _texture;
                set {
                    if (_texture != value) {
                        _texture = value;
                        _geometry.Material = _geometry.BackMaterial = value?.Diffuse.Data ?? ResourceConverts.GetRandomTexture();
                    }
                }
            }

            public bool IsVisible {
                get => _isVisible;
                set {
                    _isVisible = value;
                    VisibilityChange();
                }
            }

            public MeshData(string textureName, GeometryModel3D geometry, MtlTexture? texture = null) {
                _textureName = textureName;
                _geometry = geometry;
                _geometry.Transform = _scaleTransform = new ScaleTransform3D();
                _texture = texture;
                _isVisible = true;
            }

            public void Hide() {
                _scaleTransform.ScaleX = 0;
                _scaleTransform.ScaleY = 0;
                _scaleTransform.ScaleZ = 0;
            }

            public void Show() {
                _scaleTransform.ScaleX = 1;
                _scaleTransform.ScaleY = 1;
                _scaleTransform.ScaleZ = 1;
            }

            private void VisibilityChange() {
                if (IsVisible) {
                    Hide();
                } else {
                    Show();
                }
            }
        }

        private readonly static Material s_transparentMaterial = new DiffuseMaterial();

        private readonly Dictionary<string, MeshData> _meshes;
        private bool _isVisible;

        public PlyFile PlyFile { get; }
        public Model3DGroup Model { get; }
        public IEnumerable<string> MeshesTextureNames => _meshes.Keys;
        public bool IsVisible {
            get => _isVisible;
            set {
                if (_isVisible != value) {
                    _isVisible = value;
                    VisibleChange();
                }
            }
        }
        public bool IsEnable { get; set; }

        public PlyModel3D(PlyFile plyFile, Dictionary<string, MtlTexture?>? meshesTextures = null) {
            _meshes = [];
            PlyFile = plyFile;
            _isVisible = true;

            Model = ResourceConverts.PlyModelToModel3D(plyFile.Data, meshesTextures);

            LoadMeshes(meshesTextures);
        }

        public PlyModel3D(PlyFile plyFile, AggregateMtlFiles? mtlFiles) : this(plyFile, mtlFiles?.GetFirstMeshesTextures()) { }

        public static implicit operator Model3D?(PlyModel3D? ply) {
            return ply?.Model;
        }

        public void SetMeshTexture(string meshTextureName, MtlTexture? texture) {
            GetMesh(meshTextureName).Texture = texture;
        }

        public MtlTexture? GetMeshTexture(string meshTextureName) {
            return GetMesh(meshTextureName).Texture;
        }

        public bool CheckNoMaterial(string? meshTextureName = null) {
            if (meshTextureName != null) {
                if (GetMesh(meshTextureName) == null) {
                    return true;
                }
            } else {
                foreach (var mesh in _meshes.Values) {
                    if (mesh.Texture == null) {
                        return true;
                    }
                }
            }

            return false;
        }


        public void SetMeshVisibility(string meshTextureName, bool visible) {
            GetMesh(meshTextureName).IsVisible = visible;
        }

        public bool GetMeshVisibility(string meshTextureName) {
            return GetMesh(meshTextureName).IsVisible;
        }

        public Point3D GetCenterPoint() {
            return PlyFile.Data.Points.SwapYZ().SwapXZ().GetCenterPoint();
        }

        private MeshData GetMesh(string meshTextureName) {
            if (!_meshes.TryGetValue(meshTextureName, out var mesh)) {
                throw PlyModelException.NoContainMeshTextureName(null, meshTextureName);
            }

            return mesh;
        }

        private Dictionary<string, GeometryModel3D> LoadMeshes(Dictionary<string, MtlTexture?>? meshesTextures) {
            var meshesGeometries = new Dictionary<string, GeometryModel3D>();

            var meshEnumerator = PlyFile.Data.Meshes.Select(m => m.TextureName).GetEnumerator();
            meshEnumerator.MoveNext();

            foreach (var model in Model.Children) {
                string meshTextureName = meshEnumerator.Current;
                var modelMeshGeometry = (GeometryModel3D)model;
                var texture = meshesTextures?.GetValueOrDefault(meshTextureName);

                _meshes.Add(meshTextureName, new MeshData(meshTextureName, modelMeshGeometry, texture));

                meshEnumerator.MoveNext();
            }

            return meshesGeometries;
        }

        private void VisibleChange() {
            foreach (var mesh in _meshes.Values) {
                if (IsVisible) {
                    mesh.Show();
                } else {
                    mesh.Hide();
                }
            }
        }
    }
}
