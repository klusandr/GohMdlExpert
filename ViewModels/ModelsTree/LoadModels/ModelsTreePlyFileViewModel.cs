using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreePlyFileViewModel : ModelsTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        private readonly PlyFile _plyFile;

        public new ModelsLoadTreeViewModel Tree => (ModelsLoadTreeViewModel)base.Tree;
        public PlyFile PlyFile => _plyFile;

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);


        public ModelsTreePlyFileViewModel(PlyFile plyFile, ModelsLoadTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            _plyFile = plyFile;
            HeaderText = plyFile.Name;
            IconSource = s_iconSource;
        }

        public void LoadData() {
            if (Items.Count == 0) {
                if (Tree.HumanskinResource == null) {
                    return;
                }

                var plyModel = _plyFile.Data;

                foreach (var mesh in plyModel.Meshes) {
                    var meshesMtlFiles = Tree.HumanskinResource.GetPlyAggregateMtlFiles(_plyFile);

                    foreach (var meshMtlFile in meshesMtlFiles) {
                        AddNextNode(new ModelsTreeMashViewModel(meshMtlFile, Tree, this));
                    }
                }
            }
        }
    }
}
