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
            HeaderText = Path.GetFileNameWithoutExtension(mesh.TextureName);
            IconSource = s_iconSource;

            Mesh = mesh;
        }

    }
}
