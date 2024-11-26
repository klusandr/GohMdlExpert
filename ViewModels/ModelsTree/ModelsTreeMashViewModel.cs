using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using GohMdlExpert.Views.ModelsTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public class ModelsTreeMashViewModel : ModelsTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public PlyModel.Mesh Mesh { get; }

        public ModelsTreeMashViewModel(PlyFile plyFile, PlyModel.Mesh mesh, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            var mtlTextures = plyFile.Textures[mesh.TextureName];
            HeaderText = Path.GetFileNameWithoutExtension(mesh.TextureName);
            IconSource = s_iconSource;

            if (mtlTextures != null) {
                foreach (var mtlTexture in mtlTextures.Data) {
                    AddNextNode(new ModelsTreeTextureViewModel(mtlTexture, modelsTree, this));
                }
            }

            Mesh = mesh;
        }

    }
}
