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
    public class ModelsOverviewTreePlyViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public ModelsOverviewTreePlyViewModel(PlyModel3D plyModel, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            HeaderText = plyModel.PlyFile.Name;
            ToolTip = plyModel.PlyFile.GetFullPath();
            IconSource = s_iconSource;

            foreach (var meshTextureName in plyModel.MeshesTextureNames) {
                AddNextNode(new ModelsOverviewTreeMeshViewModel(plyModel, meshTextureName, Tree));
            }
        }
    }
} 
