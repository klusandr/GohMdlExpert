using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public class ResourceLoadTreeFileViewModel : ResourceLoadTreeItemViewModel {
        public GohResourceFile ResourceFile { get; }
        public ResourceLoadTreeFileViewModel(GohResourceFile resourceFile, TreeViewModel modelsTree) : base(modelsTree) {
            ResourceFile = resourceFile;
            
            Text = resourceFile.Name;

            Icon = IconResources.Instance.GetIcon(resourceFile switch {
                MdlFile => nameof(Resources.MdlIcon),
                PlyFile => nameof(Resources.PlyIcon),
                MtlFile => nameof(Resources.TextureIcon),
                _ => nameof(Resources.MeshIcon),
            });
        }

    }
}
