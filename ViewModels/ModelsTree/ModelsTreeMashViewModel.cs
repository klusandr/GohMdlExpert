using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using GohMdlExpert.Views.ModelsTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public class ModelsTreeMashViewModel : ModelsTreeItemViewModel {
        private static ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public ModelsTreeMashViewModel(PlyFile plyFile, PlyModel.Mesh mesh, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            var mtlFiles = modelsTree.ResourceLoader.GetPlyMtlFiles(plyFile);
            HeaderText = mesh.TextureFileName;
            IconSource = s_iconSource;

            if (mtlFiles != null) {
                var meshMtlFiles = mtlFiles
                        .Where(t => mesh.TextureFileName.Contains(t.Name))
                        .GroupBy((t) => t.Data.Diffuse)
                        .Select(t => t.First());

                foreach (var mtlFile in meshMtlFiles) {
                    AddNextNode(new ModelsTreeTextureViewModel(mtlFile, modelsTree, this));
                }

                if (Items.Count != 0 && Items.FirstOrDefault()?.DataContext is ModelsTreeTextureViewModel textureItem) {
                    textureItem.Select();
                }
            }
        }
    }
}
