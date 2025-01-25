using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert.Extensions {
    public static class BitExtension {
        /// <summary>
        /// Возвращает значение указанного бита в байте.
        /// </summary>
        /// <param name="value">Байт.</param>
        /// <param name="index">Индекс бита.</param>
        /// <returns>Значение указанного бита в байте.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static bool GetBit(this byte value, int index) {
            if (index < 0 || 7 < index) {
                throw new IndexOutOfRangeException("Индекс бита не может быть больше 7 или меньше 0.");
            }

            return (0x01 << index & value) != 0;
        }

        /// <summary>
        /// Устанавливает значение указанного бита в байте.
        /// </summary>
        /// <param name="value">Байт.</param>
        /// <param name="index">Индекс бита.</param>
        /// <param name="bitValue">Значение бита.</param>
        /// <returns>Байт, в котором указанный бит принял необходимое значение.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte SetBit(this byte value, int index, bool bitValue) {
            if (index < 0 || 7 < index) {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс бита не может быть больше 7 или меньше 0.");
            }

            if (bitValue) {
                return (byte)(value | 0x01 << (byte)index);
            } else {
                return (byte)(value & ~(0x01 << (byte)index));
            }
        }

        /// <summary>
        /// Возвращает значение бита в списке байтов.
        /// </summary>
        /// <param name="bytes">Список байтов.</param>
        /// <param name="index">Индекс бита в списке байтов.</param>
        /// <returns>Значение указанного бита.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool GetBit(this IList<byte> bytes, int index) {
            if (bytes == null) { throw new ArgumentNullException(nameof(bytes)); }

            if (index < 0 && bytes.Count * 8 <= index) {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс бита не может быть больше или равно количеству битов в байтах списка или меньше 0.");
            }

            int byteIndex = index / 8;
            int bitIndex = index % 8;

            return bytes[byteIndex].GetBit(bitIndex);
        }

        /// <summary>
        /// Устанавливает значение бита в списке байтов.
        /// </summary>
        /// <param name="bytes">Список байтов.</param>
        /// <param name="index">Индекс бита в списке байтов.</param>
        /// <param name="bitValue">Значение бита.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetBit(this IList<byte> bytes, int index, bool bitValue) {
            if (bytes == null) { throw new ArgumentNullException(nameof(bytes)); }

            if (index < 0 && bytes.Count * 8 <= index) {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс бита не может быть больше или равно количеству битов в байтах списка или меньше 0.");
            }

            int byteIndex = index / 8;
            int bitIndex = index % 8;

            bytes[byteIndex] = bytes[byteIndex].SetBit(bitIndex, bitValue);
        }
    }
}
