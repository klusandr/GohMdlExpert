using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Commands;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.LoadModels {
    public class ModelsLoadTreeMeshViewModel : ModelsLoadTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.MeshIcon);

        public AggregateMtlFile MtlFile { get; }

        public ModelsLoadTreeMeshViewModel(AggregateMtlFile mtlFile, ModelsLoadTreeViewModel modelsTree) : base(mtlFile, modelsTree) {
            Text = mtlFile.Name;
            Icon = s_icon;
            MtlFile = mtlFile;

            ContextMenuViewModel.ClearItems();
            ContextMenuViewModel
                .AddItemBuilder(new MenuItemViewModel("Copy name in buffer", new Command(() => Clipboard.SetData(DataFormats.Text, Text))));

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
