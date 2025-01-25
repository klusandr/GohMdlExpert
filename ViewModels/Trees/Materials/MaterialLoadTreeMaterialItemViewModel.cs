using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Materials {
    public class MaterialLoadTreeMaterialItemViewModel : MaterialLoadTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.TextureIcon);

        public MaterialFile MaterialFile => (MaterialFile)ResourceElement;

        public MaterialLoadTreeMaterialItemViewModel(MaterialFile materialFile, TreeViewModel modelsTree) : base(materialFile, modelsTree) {
            Icon = s_icon;
        }

        public override void AddItem(TreeItemViewModel item) {
            throw new NotImplementedException();
        }
    }
}
