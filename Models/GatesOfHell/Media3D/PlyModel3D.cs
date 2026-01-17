using System.Windows.Media.Media3D;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using WpfMvvm.Collections.ObjectModel;
using WpfMvvm.Data;

namespace GohMdlExpert.Models.GatesOfHell.Media3D {
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

        private readonly List<MeshData> _meshes;
        private readonly Model3DGroup _mainModel;
        private readonly List<Model3DGroup> _lodModels;
        private bool _isVisible;
        private int _currentLodIndex = 0;

        public int MeshesCount => _meshes.Count;

        public PlyFile PlyFile { get; }
        public ObservableList<PlyFile> LodPlyFiles { get; }

        private readonly CollectionChangeBinder<Model3DGroup> _lodPlyFilesBinding;

        public Model3DGroup Model { get; private set; }

        public IEnumerable<string> MeshesTextureNames => _meshes.Select(m => m.TextureName);
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

        public event EventHandler? ModelChanged;

        public PlyModel3D(PlyFile plyFile, Dictionary<string, MtlTexture?>? meshesTextures = null, IEnumerable<PlyFile>? lodPlyFiles = null) {
            _meshes = [];
            PlyFile = plyFile;
            _lodModels = [];
            _isVisible = true;

            LodPlyFiles = [];

            _lodPlyFilesBinding = new CollectionChangeBinder<Model3DGroup>(LodPlyFiles, _lodModels, (pF) => ResourceConverts.PlyModelToModel3D(((PlyFile)pF!).Data, meshesTextures));
            LodPlyFiles.CollectionChanged += (s, e) => ModelChanged?.Invoke(this, EventArgs.Empty);

            _mainModel = ResourceConverts.PlyModelToModel3D(plyFile.Data, meshesTextures);
            Model = _mainModel;

            if (lodPlyFiles != null) {
                foreach (var lodplyFile in lodPlyFiles) {
                    LodPlyFiles.Add(lodplyFile);
                }
            }

            LoadMeshes(meshesTextures);
        }

        public PlyModel3D(PlyFile plyFile, AggregateMtlFiles? mtlFiles, IEnumerable<PlyFile>? lodPlyFiles = null) : this(plyFile, mtlFiles?.GetFirstMeshesTextures(), lodPlyFiles) { }

        public static implicit operator Model3D?(PlyModel3D? ply) {
            return ply?.Model;
        }

        public void SetMeshTexture(string meshTextureName, MtlTexture? texture) {
            foreach (var mesh in GetMeshesByTextureName(meshTextureName)) {
                mesh.Texture = texture;
            }
        }

        public MtlTexture? GetMeshTexture(string meshTextureName) {
            return GetMeshesByTextureName(meshTextureName).FirstOrDefault()?.Texture;
        }

        public bool CheckNoMaterial(string? meshTextureName = null) {
            if (meshTextureName != null) {
                if (!GetMeshesByTextureName(meshTextureName).Any()) {
                    return true;
                }
            } else {
                foreach (var mesh in _meshes) {
                    if (mesh.Texture == null) {
                        return true;
                    }
                }
            }

            return false;
        }


        public void SetMeshVisibility(int meshIndex, bool visible) {
            GetMesh(meshIndex).IsVisible = visible;
        }

        public bool GetMeshVisibility(int meshIndex) {
            return GetMesh(meshIndex).IsVisible;
        }

        public Point3D GetCenterPoint() {
            return PlyFile.Data.Points.SwapYZ().SwapXZ().GetCenterPoint();
        }

        public void SetLodIndex(int index) {
            if (index == _currentLodIndex) {
                return;
            }

            if (index == 0) {
                Model = _mainModel;
            } else {
                if (index - 1 >= _lodModels.Count) { return; }

                Model = _lodModels[index - 1];
                for (int i = 0; i < _mainModel.Children.Count && i < Model.Children.Count; i++) {
                    var mainmodelMesh = ((GeometryModel3D)_mainModel.Children[i]);
                    var lodModelMesh = ((GeometryModel3D)Model.Children[i]);

                    lodModelMesh.Material = lodModelMesh.BackMaterial = mainmodelMesh.Material;
                }
            }

            _currentLodIndex = index;
            ModelChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetLodIndex() {
            return _currentLodIndex;
        }

        public void UnloadResource() {
            PlyFile.UnloadData();

            foreach (var item in LodPlyFiles) {
                item.UnloadData();
            }
        }

        private IEnumerable<MeshData> GetMeshesByTextureName(string meshTextureName) {
            return _meshes.Where(m => m.TextureName == meshTextureName);
        }

        private MeshData GetMesh(int meshIndex) {
            if (meshIndex < 0 || meshIndex >= MeshesCount) {
                throw PlyModelException.MeshIndexOutOfRange(PlyFile, meshIndex);
            }

            return _meshes[meshIndex];
        }

        private Dictionary<string, GeometryModel3D> LoadMeshes(Dictionary<string, MtlTexture?>? meshesTextures) {
            var meshesGeometries = new Dictionary<string, GeometryModel3D>();

            var meshEnumerator = PlyFile.Data.Meshes.Select(m => m.TextureName).GetEnumerator();
            meshEnumerator.MoveNext();

            foreach (var model in Model.Children) {
                string meshTextureName = meshEnumerator.Current;
                var modelMeshGeometry = (GeometryModel3D)model;
                var texture = meshesTextures?.GetValueOrDefault(meshTextureName);

                _meshes.Add(new MeshData(meshTextureName, modelMeshGeometry, texture));

                meshEnumerator.MoveNext();
            }

            return meshesGeometries;
        }

        private void VisibleChange() {
            foreach (var mesh in _meshes) {
                if (IsVisible) {
                    mesh.Show();
                } else {
                    mesh.Hide();
                }
            }
        }
    }
}
