using GohMdlExpert.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
