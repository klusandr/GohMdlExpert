﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;

namespace GohMdlExpert.Models.GatesOfHell.Resources.Data {
    public class MtlTexture {
        private class EqualsCompare : IEqualityComparer<MtlTexture> {
            public bool Equals(MtlTexture? x, MtlTexture? y) {
                return x?.Equals(y) ?? y == null;
            }

            public int GetHashCode([DisallowNull] MtlTexture obj) {
                return obj.GetHashCode();
            }
        }

        public MaterialFile Diffuse { get; set; }
        public MaterialFile? Bump { get; set; }
        public MaterialFile? Specular { get; set; }
        public Color? Color { get; set; }

        public MtlTexture(MaterialFile diffuse) {
            Diffuse = diffuse;
        }

        public static IEqualityComparer<MtlTexture> GetEqualityComparer() => new EqualsCompare();

        public static MtlTexture NullTexture { get; } = new MtlTexture(new NullMaterialFile() { Name = string.Empty });

        public override bool Equals(object? obj) {
            if (obj == null) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj is MtlTexture mtl) {
                return Diffuse.Equals(mtl.Diffuse)
                    && (Bump?.Equals(mtl.Bump) ?? mtl.Bump == null)
                    && (Specular?.Equals(mtl.Specular) ?? mtl.Specular == null)
                    && (Color?.Equals(mtl.Color) ?? mtl.Color == null);
            }

            return false;
        }

        public override int GetHashCode() {
            return Diffuse.GetHashCode()
                + (Bump?.GetHashCode() ?? 0)
                + (Specular?.GetHashCode() ?? 0)
                + (Color?.GetHashCode() ?? 0);
        }

        public MtlTexture Clone() {
            return (MtlTexture)MemberwiseClone();
        }
    }
}
