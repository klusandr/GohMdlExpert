using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Media3D;
using GohMdlExpert.Properties;
using GohMdlExpert.Extensions;
using System.Windows.Input;
using System.ComponentModel;
using WpfMvvm.Collections;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreePlyViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_icon = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveModel);

        public ModelsOverviewTreePlyViewModel(PlyModel3D plyModel, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            Text = plyModel.PlyFile.Name;
            ToolTip = plyModel.PlyFile.GetFullPath();
            Icon = s_icon;
            ContextMenuViewModel.AddItem(new MenuItemViewModel(RemoveCommand, "Remove"));
            IsVisibleActive = true;
            IsEnableCheckActive = true;

            PropertyChangeHandler.AddHandler(nameof(IsVisible), VisibleChangedHandler);

            foreach (var meshTextureName in plyModel.MeshesTextureNames) {
                AddItem(new ModelsOverviewTreeMeshViewModel(plyModel, meshTextureName, Tree));
            }

            PlyModel = plyModel;
        }

        public PlyModel3D PlyModel { get; }

        private void RemoveModel() {
            Tree.Models3DViewModel.RemoveModel(PlyModel);
        }

        private void VisibleChangedHandler(object? sender, PropertyChangedEventArgs e) {
            PlyModel.IsVisible = IsVisible;
        }
    }
} 
