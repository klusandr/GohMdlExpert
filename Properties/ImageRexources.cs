using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;

namespace GohMdlExpert.Properties {
    internal static class ImageResources {
        private static readonly Dictionary<string, BitmapImage> s_imageSources;

        static ImageResources() {
            s_imageSources = [];
        }

        public static BitmapImage GetIcon(string resourceName) {
            if (!s_imageSources.TryGetValue(resourceName, out BitmapImage? bitmapImage)) {
                ThrowCheckResource(resourceName, typeof(byte[]));
                bitmapImage = new BitmapImage().FromByteArray((byte[])Resources.ResourceManager.GetObject(resourceName)!);
                s_imageSources.Add(resourceName, bitmapImage);
            }

            return bitmapImage;
        }

        private static void ThrowCheckResource(string resourceName, Type type) {
            if ( typeof(Resources).GetProperty(resourceName, type) == null) {
                throw new Exception($"App resource load error. Resource \"{resourceName}\" with type \"{type.Name}\".");
            }
        }
    }
}
