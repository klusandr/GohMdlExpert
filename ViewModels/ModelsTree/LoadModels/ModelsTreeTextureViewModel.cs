using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    internal class ModelsTreeTextureViewModel : ModelsTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.TextureIcon);

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);

        public MtlTexture MtlTexture { get; }

        public ModelsTreeTextureViewModel(MtlTexture mtlTexture, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            HeaderText = mtlTexture.Diffuse.Name;
            ToolTip = mtlTexture.Diffuse.GetFullPath();
            IconSource = s_iconSource;
            MtlTexture = mtlTexture;
        }
    }
}
