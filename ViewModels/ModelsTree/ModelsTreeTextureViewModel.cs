using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree {
    internal class ModelsTreeTextureViewModel : ModelsTreeItemViewModel {
        private static ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.TextureIcon);
        private readonly MtlFile _mtlFile;

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);

        public ModelsTreeTextureViewModel(MtlFile mtlFile, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            _mtlFile = mtlFile;
            HeaderText = mtlFile.Data.Diffuse.Name;
            ToolTip = mtlFile.Data.Diffuse.GetFullPath();
            IconSource = s_iconSource;
        }

        public void Approve() {
            if (!IsApproved) {
                ModelsTree.Models3DView.Adder.SetModelTexture(_mtlFile);
                IsApproved = true;
            }
        }
    }
}
