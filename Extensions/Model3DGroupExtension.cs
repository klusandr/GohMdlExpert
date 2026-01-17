using System.Windows.Media.Media3D;

namespace GohMdlExpert.Extensions {
    public static class Model3DGroupExtension {
        public static void SetSelectMaterial(this Model3DGroup model3DGroup) {
            foreach (var model in model3DGroup.Children) {
                if (model is GeometryModel3D model3D) {
                    model3D.SetSelectMaterial();
                }
            }
        }

        public static void ClearSelectMaterial(this Model3DGroup model3DGroup) {
            foreach (var model in model3DGroup.Children) {
                if (model is GeometryModel3D model3D) {
                    model3D.ClearSelectMaterial();
                }
            }
        }
    }
}
