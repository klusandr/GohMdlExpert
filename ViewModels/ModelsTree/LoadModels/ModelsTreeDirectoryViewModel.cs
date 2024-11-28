using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using GohMdlExpert.Views.ModelsTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels
{
    public class ModelsTreeDirectoryViewModel : ModelsTreeItemViewModel {
        private static ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.DirectoryIcon);

        private readonly GohResourceDirectory _directory;

        public new ModelsLoadTreeViewModel Tree => (ModelsLoadTreeViewModel)base.Tree;

        public override ICommand? DoubleClickCommand => CommandManager.GetCommand(LoadData);

        public ModelsTreeDirectoryViewModel(GohResourceDirectory directory, ModelsLoadTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) : base(modelsTree, parent) {
            HeaderText = directory.Name;
            _directory = directory;
            IconSource = s_iconSource;
        }

        public void LoadData()
        {
            Items.Clear();

            var directories = _directory.GetDirectories();
            var plyFiles = _directory.GetFiles().OfType<PlyFile>().Where(f => !f.Name.Contains("lod"));

            foreach (var directory in directories)
            {
                AddNextNode(new ModelsTreeDirectoryViewModel(directory, Tree, this));
            }

            foreach (var plyFile in plyFiles)
            {
                AddNextNode(new ModelsTreePlyFileViewModel(plyFile, Tree, this));
            }
        }
    }
}
