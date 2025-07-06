using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NaturalSort.Extension;

namespace GohMdlExpert.Models.GatesOfHell.Extensions {
    public static class CollectionsExtensions {
        private static readonly NaturalSortComparer s_naturalComparer = StringComparer.OrdinalIgnoreCase.WithNaturalSort();

        public static int FindIndex<T>(this IEnumerable<T> items, T item) {
            int currentIndex = 0;
            int index = -1;

            foreach (T currentItem in items) {
                if (currentItem?.Equals(item) ?? false) {
                    index = currentIndex;
                    break;
                }

                currentIndex++;
            }

            return index;
        }

        public static int FindLastIndex<T>(this IEnumerable<T> items, T item) {
            int currentIndex = items.Count() - 1;
            int index = -1;

            foreach (T currentItem in items.Reverse()) {
                if (currentItem?.Equals(item) ?? false) {
                    index = currentIndex;
                    break;
                }

                currentIndex--;
            }

            return index;
        }

        public static IEnumerable<T> OrderByNature<T>(this IEnumerable<T> items, Func<T, string> selector) {
            return items.OrderBy(i => selector(i).ToLower(), s_naturalComparer);
        } 
    }
}
