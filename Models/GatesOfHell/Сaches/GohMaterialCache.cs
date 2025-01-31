using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Сaches {
    public class GohMaterialCache {
        private readonly Dictionary<string, WeakReference<DiffuseMaterial>> _materialsCache = [];

        public DiffuseMaterial? this[string key] { get => GetMaterial(key); set => SetMaterial(key, value); }

        public GohMaterialCache() { }

        public DiffuseMaterial? GetMaterial(string materialName) {
            if (_materialsCache.TryGetValue(materialName, out var reference)) {
                if (!reference.TryGetTarget(out var material)) {
                    _materialsCache.Remove(materialName);
                }

                return material;
            }

            return null;
        }

        public bool TryGetMaterial(string materialName, [MaybeNullWhen(false)] out DiffuseMaterial material) {
            material = GetMaterial(materialName);

            return material != null;
        }

        public void SetMaterial(string materialName, DiffuseMaterial? material) {
            if (material != null) {
                if (_materialsCache.TryGetValue(materialName, out var reference)) {
                    reference.SetTarget(material);
                } else {
                    _materialsCache.Add(materialName, new WeakReference<DiffuseMaterial>(material));
                }
            } else {
                _materialsCache.Remove(materialName);
            }
        }
    }
}
