using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Policy;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Data {
    public class MtlTexture {
        private MaterialFile? _diffuse;
        private readonly string? _diffusePath;
        private readonly string? _specularPath;
        private readonly string? _bumpPath;

        private class EqualsCompare : IEqualityComparer<MtlTexture> {
            public bool Equals(MtlTexture? x, MtlTexture? y) {
                return x?.Equals(y) ?? y == null;
            }

            public int GetHashCode([DisallowNull] MtlTexture obj) {
                return obj.GetHashCode();
            }
        }

        public static MtlTexture NullTexture { get; } = new MtlTexture(new NullMaterialFile() { Name = string.Empty });

        public string DiffusePath => _diffuse?.GetFullPath() ?? _diffusePath!;
        public string? BumpPath => Bump?.GetFullPath() ?? _bumpPath; 
        public string? SpecularPath => Specular?.GetFullPath() ?? _specularPath;

        public MaterialFile Diffuse { get => _diffuse ?? throw TextureException.MaterialsNotInitialize(this); set => _diffuse = value; }
        public MaterialFile? Bump { get; set; }
        public MaterialFile? Specular { get; set; }
        public Color? Color { get; set; }

        public bool IsMaterialsInitialize => _diffuse != null;

        public MtlTexture(MaterialFile diffuse) {
            Diffuse = diffuse;
        }

        public MtlTexture(string DiffusePath, string? BumpPath, string? SpecularPath) {
            _diffusePath = DiffusePath;
            _bumpPath = BumpPath;
            _specularPath = SpecularPath;
        }

        public static IEqualityComparer<MtlTexture> GetEqualityComparer() => new EqualsCompare();

        public override bool Equals(object? obj) {
            if (obj == null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj is MtlTexture mtl) {
                return GetHashCode() == mtl.GetHashCode();
            }

            return false;
        }

        public override int GetHashCode() {
            if (IsMaterialsInitialize) {
                return HashCode.Combine(Diffuse.GetHashCode(), Bump?.GetHashCode(), Specular?.GetHashCode(), Color?.GetHashCode());
            } else {
                return HashCode.Combine(DiffusePath.GetHashCode(), BumpPath?.GetHashCode(), SpecularPath?.GetHashCode(), Color?.GetHashCode());
            }
        }

        public MtlTexture Clone() {
            return (MtlTexture)MemberwiseClone();
        }
    }
}
