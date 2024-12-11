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

namespace GohMdlExpert.ViewModels.ModelsTree.OverviewModels {
    public class ModelsOverviewTreePlyViewModel : ModelsOverviewTreeItemViewModel {
        private static readonly ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        public ICommand RemoveCommand => CommandManager.GetCommand(RemoveModel);

        public ModelsOverviewTreePlyViewModel(PlyModel3D plyModel, ModelsOverviewTreeViewModel modelsTree) : base(modelsTree) {
            HeaderText = plyModel.PlyFile.Name;
            ToolTip = plyModel.PlyFile.GetFullPath();
            IconSource = s_iconSource;
            ContextMenuCommands.Add("Remove", RemoveCommand);

            PropertyChangeHandler.AddHandler(nameof(IsVisible), VisibleChangedHandler);

            foreach (var meshTextureName in plyModel.MeshesTextureNames) {
                AddNextNode(new ModelsOverviewTreeMeshViewModel(plyModel, meshTextureName, Tree));
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
