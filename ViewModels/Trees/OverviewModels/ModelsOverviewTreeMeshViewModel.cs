using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreeMeshViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public ModelsOverviewTreeMeshViewModel(PlyModel3D plyModel, string meshTextureName, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Icon = s_icon;
            var texture = plyModel.GetMeshTexture(meshTextureName);
            Text = $"{meshTextureName} [{texture?.Diffuse.Name ?? "null"}]";
            IsVisibleActive = true;
        }
    }
}
