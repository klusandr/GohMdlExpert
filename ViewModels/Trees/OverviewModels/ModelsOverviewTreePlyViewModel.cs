using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreePlyViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveModel);

        public ModelsOverviewTreePlyViewModel(PlyModel3D plyModel, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Text = plyModel.PlyFile.Name;
            ToolTip = plyModel.PlyFile.GetFullPath();
            Icon = s_icon;
            IsVisibleActive = true;
            IsEnableCheckActive = true;
            ContextMenuViewModel.AddItem(new MenuItemViewModel("Remove", RemoveCommand));

            PropertyChangeHandler
                .AddHandlerBuilder(nameof(IsVisible), VisibleChangedHandler)
                .AddHandlerBuilder(nameof(IsSelected), IsSelectedChangeHandler);

            foreach (var meshTextureName in plyModel.MeshesTextureNames) {
                AddItem(new ModelsOverviewTreeMeshViewModel(plyModel, meshTextureName, Tree));
            }

            PlyModel = plyModel;
        }

        public PlyModel3D PlyModel { get; }

        private void RemoveModel() {
            Tree.Models3DViewModel.RemoveModel(PlyModel);
        }

        private void IsSelectedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsSelected) {
                Tree.LodListViewModel.Items = Tree.Models3DViewModel.GetPlyModelLodFiles(PlyModel);
            } else {
                Tree.LodListViewModel.Items = null;
            }
        }

        private void VisibleChangedHandler(object? sender, PropertyChangedEventArgs e) {
            PlyModel.IsVisible = IsVisible;
        }
    }
}
