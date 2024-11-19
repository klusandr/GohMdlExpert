using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using GohMdlExpert.Views.ModelsTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public class ModelsTreePlyViewModel : ModelsTreeItemViewModel {
        private static ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.PlyIcon);

        private readonly PlyFile _plyFile;

        public override ICommand DoubleClickCommand => CommandManager.GetCommand(Select);

        public ModelsTreePlyViewModel(PlyFile plyFile, ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            _plyFile = plyFile;
            HeaderText = plyFile.Name;
            IconSource = s_iconSource;
        }

        public void Select() {
            ModelsTree.Models3DView.Adder.SetModel(_plyFile);
            IsApproved = true;
            foreach (var mesh in _plyFile.Data.Meshes) {
                AddNextNode(new ModelsTreeMashViewModel(_plyFile, mesh, ModelsTree, this));
            }
        }
    }
}
