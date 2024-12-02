using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreeTextureViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.TextureIcon);

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Approve);

        public MtlTexture MtlTexture { get; }
        public ModelsTreeMeshViewModel ModelsTreeMesh => (ModelsTreeMeshViewModel)Parent!;

        public ModelsTreeTextureViewModel(MtlTexture mtlTexture, ModelsLoadTreeViewModel modelsTree, ModelsTreeMeshViewModel parent) : base(mtlTexture.Diffuse, modelsTree) {
            IconSource = s_iconSource;
            MtlTexture = mtlTexture;
            Parent = parent;
        }

        public override void LoadData() { }

        public override void Approve() {
            if (!IsApproved) {
                base.Approve();
                Tree.ModelsAdder.SelectModelMeshTexture(((ModelsTreeMeshViewModel)Parent!).MtlFile.Name, MtlTexture);
                Tree.ApprovedTextureItems.Add(this);
            }
        }

        public override void CancelApprove() {
            if (IsApproved) {
                base.CancelApprove();
                Tree.ApprovedTextureItems.Remove(this);
            }
            
        }
    }
}
