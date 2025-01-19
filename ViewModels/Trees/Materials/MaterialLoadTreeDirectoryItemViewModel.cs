using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;
using GohMdlExpert.Extensions;

namespace GohMdlExpert.ViewModels.Trees.Materials
{
    public class MaterialLoadTreeDirectoryItemViewModel : MaterialLoadTreeItemViewModel
    {
        private static ImageSource s_icon = new BitmapImage().FromByteArray(Resources.DirectoryIcon);
        public GohResourceDirectory ResourceDirectory => (GohResourceDirectory)ResourceElement;

        public MaterialLoadTreeDirectoryItemViewModel(GohResourceDirectory resourceDirectory, TreeViewModel modelsTree) : base(resourceDirectory, modelsTree)
        {
            Icon = s_icon;
            LoadData();
        }

        private void LoadData()
        {
            foreach (var item in ResourceDirectory.GetDirectories())
            {
                AddItem(new MaterialLoadTreeDirectoryItemViewModel(item, Tree));
            }

            foreach (var item in ResourceDirectory.Items.OfType<MaterialFile>())
            {
                AddItem(new MaterialLoadTreeMaterialItemViewModel(item, Tree));
            }
        }
    }
}
