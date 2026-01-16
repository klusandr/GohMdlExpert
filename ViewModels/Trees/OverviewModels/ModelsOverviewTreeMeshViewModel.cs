using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreeMeshViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MeshIcon);
        private readonly PlyModel3D _plyModel;
        private readonly string _meshTextureName;

        public ModelsOverviewTreeMeshViewModel(PlyModel3D plyModel, string meshTextureName, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            _plyModel = plyModel;
            _meshTextureName = meshTextureName;
            Icon = s_icon;
            var texture = plyModel.GetMeshTexture(meshTextureName);
            Text = GetFullText();
            IsVisibleActive = true;

            WeakEventManager<HumanskinMdlOverviewViewModel, EventArgs>.AddHandler(Tree.Models3DViewModel, nameof(HumanskinMdlOverviewViewModel.UpdatedTextures), UpdatedTexturesHandler);
        }

        private string GetFullText() {
            var texture = _plyModel.GetMeshTexture(_meshTextureName);
            return $"{_meshTextureName} [{texture?.Diffuse.Name ?? "null"}]";
        }

        private void UpdatedTexturesHandler(object? sender, EventArgs e) {
            Text = GetFullText();
        }
    }
}
