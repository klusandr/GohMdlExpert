using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.Trees.LoadModels
{
    public class ModelsLoadTreeTextureViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.TextureIcon);

        public MtlTexture MtlTexture { get; }
        public ModelsLoadTreeMeshViewModel Mesh => (ModelsLoadTreeMeshViewModel)Parent!;

        public ModelsLoadTreeTextureViewModel(MtlTexture mtlTexture, ModelsLoadTreeViewModel modelsTree, ModelsLoadTreeMeshViewModel parent) : base(mtlTexture.Diffuse, modelsTree) {
            Icon = s_icon;
            MtlTexture = mtlTexture;
            Parent = parent;
        }

        public override void LoadData() { }

        public override void Approve() {
            if (!IsApproved) {
                base.Approve();
                Tree.ModelsAdder.SelectModelMeshTexture(Mesh.MtlFile.Name, MtlTexture);
            }
        }
    }
}
