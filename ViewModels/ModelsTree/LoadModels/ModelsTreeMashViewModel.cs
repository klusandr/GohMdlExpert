using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreeMashViewModel : ModelsTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public PlyAggregateMtlFile MtlFile { get; }

        public ModelsTreeMashViewModel(PlyAggregateMtlFile mtlFile, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            HeaderText = mtlFile.Name;
            IconSource = s_iconSource;
            MtlFile = mtlFile;

            foreach (var texture in mtlFile.Data) {
                AddNextNode(new ModelsTreeTextureViewModel(texture, Tree, this));
            }
        }

    }
}
