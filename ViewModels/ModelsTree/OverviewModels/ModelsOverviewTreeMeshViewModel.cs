using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Properties;
using GohMdlExpert.Extensions;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreeMeshViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public ModelsOverviewTreeMeshViewModel(PlyModel3D plyModel, string meshTextureName, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Icon = s_icon;
            var texture = plyModel.GetMeshTexture(meshTextureName);
            Text =  $"{meshTextureName} [{texture?.Diffuse.Name ?? "null"}]";
            IsVisibleActive = true;
        }
    }
} 
