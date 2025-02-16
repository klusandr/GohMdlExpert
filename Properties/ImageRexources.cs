using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;

namespace GohMdlExpert.Properties {
    public class IconResources {
        private readonly Dictionary<string, BitmapImage> _imageSources;
        private static IconResources? s_instance;

        public static IconResources Instance => s_instance ??= new IconResources();

        private IconResources() {
            _imageSources = [];
        }

        public ImageSource GetIcon(string resourceName) {
            if (!_imageSources.TryGetValue(resourceName, out BitmapImage? bitmapImage)) {
                var resource = Resources.ResourceManager.GetObject(resourceName);

                if (resource is byte[] iconResource) {
                    bitmapImage = new BitmapImage().FromByteArray(iconResource);
                    _imageSources.Add(resourceName, bitmapImage);
                } else {
                    throw new Exception($"App resource load error. Resource \"{resourceName}\" with type \"{typeof(byte[]).Name}\".");
                }
            }

            return bitmapImage;
        }
    }
}
