using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Exceptions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreePlyFileViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public PlyFile PlyFile { get; }
        public PlyAggregateMtlFiles? AggregateMtlFiles { get; private set; }

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);

        public ModelsTreePlyFileViewModel(PlyFile plyFile, ModelsLoadTreeViewModel modelsTree) : base(plyFile, modelsTree) {
            PlyFile = plyFile;
            IconSource = s_iconSource;
        }

        public override void LoadData() {
            if (Items.Count != 0 || Tree.HumanskinResource == null) {
                return;
            }

            AggregateMtlFiles = new PlyAggregateMtlFiles(PlyFile, Tree.HumanskinResource);

            foreach (var mtlTexture in AggregateMtlFiles.SelectMany(a => a.Data)) {
                Tree.TextureProvider.SetTextureMaterialsFullPath(mtlTexture);
            }

            foreach (var mtlFile in AggregateMtlFiles) {
                AddNextNode(new ModelsTreeMeshViewModel(mtlFile, Tree));
            }
        }

        public override void Approve() {
            if (!IsApproved) {
                try {
                    LoadData();
                    Tree.ModelsAdder.SetModel(PlyFile, AggregateMtlFiles);
                } catch (GohResourceFileException) {
                    throw;
                } finally {
                    base.Approve();
                    Tree.ApprovedPlyItem = this;
                }
            }
        }

        public override void CancelApprove() {
            if (IsApproved) {
                base.CancelApprove();
                ClearData();
                Tree.ApprovedTextureItems.Clear();
            }
        }
    }
}
