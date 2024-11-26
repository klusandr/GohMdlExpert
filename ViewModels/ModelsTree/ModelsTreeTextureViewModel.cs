using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
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
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.TextureIcon);
        
        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);

        public ModelsTreeTextureViewModel(MtlTexture mtlTexture, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            HeaderText = mtlTexture.Diffuse.Name;
            ToolTip = mtlTexture.Diffuse.GetFullPath();
            IconSource = s_iconSource;
        }
    }
}
