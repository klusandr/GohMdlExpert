using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsLoadTreeMeshViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public PlyAggregateMtlFile MtlFile { get; }

        public ModelsLoadTreeMeshViewModel(PlyAggregateMtlFile mtlFile, ModelsLoadTreeViewModel modelsTree) : base(mtlFile, modelsTree) {
            Text = mtlFile.Name;
            Icon = s_icon;
            MtlFile = mtlFile;
            LoadData();
        }

        public override void LoadData() {
            if (Items.Count != 0) {
                return;
            }

            foreach (var texture in MtlFile.Data) {
                var textureItem = new ModelsLoadTreeTextureViewModel(texture, Tree, this);

                textureItem.PropertyChangeHandler.AddHandler(nameof(textureItem.IsApproved), TextureItemApprovedChange);

                AddItem(textureItem);
            }
        }

        private void TextureItemApprovedChange(object? sender, PropertyChangedEventArgs e) {
            var textureItem = (ModelsLoadTreeTextureViewModel)sender!;

            foreach (var item in Items.OfType<ModelsLoadTreeTextureViewModel>()) {
                if (item != textureItem) {
                    item.CancelApprove();
                }
            }
        }

        public override void Approve() { }
    }
}
