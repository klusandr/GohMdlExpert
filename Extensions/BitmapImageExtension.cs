using System.IO;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.Extensions {
    public static class BitmapImageExtension {
        public static BitmapImage FromByteArray(this BitmapImage bitmap, byte[] bytes) {
            using var d = new MemoryStream(bytes);

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = d;
            bitmap.EndInit();

            return bitmap;
        }

        public static BitmapImage FromStream(this BitmapImage bitmap, Stream stream) {
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
        }
    }
}
