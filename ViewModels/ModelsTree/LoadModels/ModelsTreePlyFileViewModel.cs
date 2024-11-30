using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreePlyFileViewModel : ModelsTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public PlyFile PlyFile { get; }
        public PlyAggregateMtlFiles? AggregateMtlFiles { get; private set; }
        public new ModelsLoadTreeViewModel Tree => (ModelsLoadTreeViewModel)base.Tree;

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);

        public ModelsTreePlyFileViewModel(PlyFile plyFile, ModelsLoadTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            PlyFile = plyFile;
            HeaderText = plyFile.Name;
            IconSource = s_iconSource;
        }

        public void LoadData() {
            if (Items.Count == 0) {
                if (Tree.HumanskinResource == null) {
                    return;
                }

                AggregateMtlFiles = new PlyAggregateMtlFiles(PlyFile, Tree.HumanskinResource);

                foreach (var mtlTexture in AggregateMtlFiles.SelectMany(a => a.Data)) {
                    Tree.TextureProvider.SetTextureMaterialsFullPath(mtlTexture);
                }

                foreach (var mtlFile in AggregateMtlFiles) {
                    AddNextNode(new ModelsTreeMashViewModel(mtlFile, Tree, this));
                }
            }
        }
    }
}
