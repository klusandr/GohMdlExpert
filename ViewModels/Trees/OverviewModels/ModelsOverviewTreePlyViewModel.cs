using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Commands;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.OverviewModels {
    public class ModelsOverviewTreePlyViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public PlyModel3D PlyModel { get; }

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveModel);
        public ICommand ShowCommand => CommandManager.GetCommand(Show, canExecute: (_) => !IsVisible);
        public ICommand HideCommand => CommandManager.GetCommand(Hide, canExecute: (_) => IsVisible);
        public ICommand SetFocusCommand => CommandManager.GetCommand(SetFocus);

        public ModelsOverviewTreePlyViewModel(PlyModel3D plyModel, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Text = plyModel.PlyFile.Name;
            ToolTip = plyModel.PlyFile.GetFullPath();
            Icon = s_icon;
            IsVisibleActive = true;
            IsEnableCheckActive = true;
            ContextMenuViewModel
                .AddItemBuilder(new MenuItemViewModel("Show", ShowCommand) { VisibleFromEnable = true })
                .AddItemBuilder(new MenuItemViewModel("Hide", HideCommand) { VisibleFromEnable = true })
                .AddItemBuilder(new MenuItemViewModel("Set focus", SetFocusCommand))
                .AddItemBuilder(new MenuItemViewModel("Remove", RemoveCommand))
                ;

            PropertyChangeHandler
                .AddHandlerBuilder(nameof(IsVisible), VisibleChangedHandler)
                .AddHandlerBuilder(nameof(IsSelected), IsSelectedChangeHandler);

            foreach (var meshTextureName in plyModel.MeshesTextureNames) {
                AddItem(new ModelsOverviewTreeMeshViewModel(plyModel, meshTextureName, Tree));
            }

            PlyModel = plyModel;
        }

        private void SetFocus() {
            Tree.Models3DViewModel.FocusablePlyModel = PlyModel;
        }

        private void Show() {
            IsVisible = true;
        }

        private void Hide() {
            IsVisible = false;
        }

        private void RemoveModel() {
            Tree.Models3DViewModel.RemoveModel(PlyModel);
        }

        private void IsSelectedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsSelected) {
                Tree.LodListViewModel.Items = Tree.Models3DViewModel.GetPlyModelLodFiles(PlyModel);
                PlyModel.Model.SetSelectMaterial();
            } else {
                Tree.LodListViewModel.Items = null;
                PlyModel.Model.ClearSelectMaterial();
            }
        }

        private void VisibleChangedHandler(object? sender, PropertyChangedEventArgs e) {
            (ShowCommand as Command)?.OnCanExecuteChanged();
            (HideCommand as Command)?.OnCanExecuteChanged();
            PlyModel.IsVisible = IsVisible;
        }
    }
}
