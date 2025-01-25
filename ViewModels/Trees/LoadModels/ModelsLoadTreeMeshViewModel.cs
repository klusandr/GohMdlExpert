using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Properties;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.LoadModels
{
    public class ModelsLoadTreeMeshViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public AggregateMtlFile MtlFile { get; }

        public ICommand SetDefaultMaterialCommand => CommandManager.GetCommand(SetDefaultMaterial);

        public ModelsLoadTreeMeshViewModel(AggregateMtlFile mtlFile, ModelsLoadTreeViewModel modelsTree) : base(mtlFile, modelsTree) {
            Text = mtlFile.Name;
            Icon = s_icon;
            MtlFile = mtlFile;

            ContextMenuViewModel.ClearItems();
            ContextMenuViewModel.AddItem(new MenuItemViewModel("Set default texture", SetDefaultMaterialCommand));

            LoadData();
        }

        public override void LoadData() {
            if (Items.Any()) {
                return;
            }

            foreach (var texture in MtlFile.Data) {
                var textureItem = new ModelsLoadTreeTextureViewModel(texture, Tree, this);

                textureItem.PropertyChangeHandler.AddHandler(nameof(textureItem.IsApproved), TextureItemApprovedChange);

                AddItem(textureItem);
            }
        }

        public override void Approve() { }

        private void TextureItemApprovedChange(object? sender, PropertyChangedEventArgs e) {
            var textureItem = (ModelsLoadTreeTextureViewModel)sender!;

            foreach (var item in Items.OfType<ModelsLoadTreeTextureViewModel>()) {
                if (item != textureItem) {
                    item.CancelApprove();
                }
            }
        }


        private void SetDefaultMaterial(object? obj) {
#warning Доделать...
            throw new NotImplementedException("Скоро будет...");
        }
    }
}
