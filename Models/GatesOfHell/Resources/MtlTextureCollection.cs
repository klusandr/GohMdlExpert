using System.Collections;
using System.Linq;

namespace GohMdlExpert.Models.GatesOfHell.Resources {
    public class MtlTextureCollection : ICollection<MtlTexture> {
        public enum CollectionEquality {
            Full,
            Partial,
            No
        }

        private readonly List<MtlTexture> _mtlTextures;

        public MtlTextureCollection() {
            _mtlTextures = [];
        }

        public MtlTextureCollection(IEnumerable<MtlTexture> mtlTextures) {
            _mtlTextures = new(mtlTextures);
        }

        public CollectionEquality Equals(MtlTextureCollection mtlTextures) {
            bool fullEquality = true;
            bool partialEquality = false;

            foreach (var mtlTexture in mtlTextures) {
                if (partialEquality && !fullEquality) {
                    return CollectionEquality.Partial;
                }

                if (this.Any(mtlTexture.Equals)) {
                    partialEquality = true;
                } else {
                    fullEquality = false;
                }
            }

            return fullEquality ? CollectionEquality.Full : CollectionEquality.No;
        }

        public MtlTextureCollection GetCompatibleCollection(MtlTextureCollection mtlTextures) {
            return new(this.Intersect(mtlTextures));
        }

        #region ICollection
        public int Count => ((ICollection<MtlTexture>)_mtlTextures).Count;

        public bool IsReadOnly => ((ICollection<MtlTexture>)_mtlTextures).IsReadOnly;

        public void Add(MtlTexture item) {
            ((ICollection<MtlTexture>)_mtlTextures).Add(item);
        }

        public void Clear() {
            ((ICollection<MtlTexture>)_mtlTextures).Clear();
        }

        public bool Contains(MtlTexture item) {
            return ((ICollection<MtlTexture>)_mtlTextures).Contains(item);
        }

        public void CopyTo(MtlTexture[] array, int arrayIndex) {
            ((ICollection<MtlTexture>)_mtlTextures).CopyTo(array, arrayIndex);
        }

        public IEnumerator<MtlTexture> GetEnumerator() {
            return ((IEnumerable<MtlTexture>)_mtlTextures).GetEnumerator();
        }

        public bool Remove(MtlTexture item) {
            return ((ICollection<MtlTexture>)_mtlTextures).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_mtlTextures).GetEnumerator();
        }
        #endregion
    }
}
