using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public class ResourceLoadTreeFileViewModel : ResourceLoadTreeItemViewModel {
        public GohResourceFile ResourceFile => (GohResourceFile)ResourceElement;
        public ResourceLoadTreeFileViewModel(GohResourceFile resourceFile, TreeViewModel modelsTree) : base(resourceFile, modelsTree) {
            Icon = IconResources.Instance.GetIcon(resourceFile switch {
                MdlFile => nameof(Resources.MdlIcon),
                PlyFile => nameof(Resources.PlyIcon),
                MtlFile => nameof(Resources.TextureIcon),
                _ => nameof(Resources.MeshIcon),
            });
        }

    }
}
