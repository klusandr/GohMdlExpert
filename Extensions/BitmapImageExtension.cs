using System.IO;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.Extensions {
    public static class BitmapImageExtension {
        public static BitmapImage FromByteArray(this BitmapImage bitmap, byte[] bytes) {
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(bytes);
            bitmap.EndInit();

            return bitmap;
        }
    }
}
