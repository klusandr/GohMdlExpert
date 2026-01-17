using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Textures {
    public class TextureLoadTreeTextureViewModel : TextureLoadTreeItemViewModel {
        public MtlFile MtlFile { get; }

        public TextureLoadTreeTextureViewModel(MtlFile mtlFile, TreeViewModel modelsTree) : base(modelsTree) {
            Text = $"{mtlFile.Name}[{mtlFile.Data.Diffuse.Name}]";
            Icon = IconResources.Instance.GetIcon(nameof(Resources.TextureIcon));
            ToolTip = mtlFile.GetFullPath();
            MtlFile = mtlFile;

            DoubleClickCommand = CommandManager.GetCommand(Tree.OnTextureApply);
        }

    }
}
