using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreeMeshViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public PlyAggregateMtlFile MtlFile { get; }

        public ModelsTreeMeshViewModel(PlyAggregateMtlFile mtlFile, ModelsLoadTreeViewModel modelsTree) : base(mtlFile, modelsTree) {
            HeaderText = mtlFile.Name;
            IconSource = s_iconSource;
            MtlFile = mtlFile;
            LoadData();
        }

        public override void LoadData() {
            if (Items.Count != 0) {
                return;
            }

            foreach (var texture in MtlFile.Data) {
                AddNextNode(new ModelsTreeTextureViewModel(texture, Tree, this));
            }
        }
    }
}
