using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace GohMdlExpert.Models.GatesOfHell.Media3D {
    public class PlyModel3DToModel3DConverter : IValueConverter {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is IEnumerable<PlyModel3D> plyCollection) {
                return new Model3DCollection(plyCollection.Select(p => (Model3D?)p));
            } else if (value is PlyModel3D ply) {
                return (Model3D?)ply;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
