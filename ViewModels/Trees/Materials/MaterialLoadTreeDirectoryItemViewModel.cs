using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Materials {
    public class MaterialLoadTreeDirectoryItemViewModel : MaterialLoadTreeItemViewModel {
        private static ImageSource s_icon = new BitmapImage().FromByteArray(Resources.DirectoryIcon);
        public GohResourceDirectory ResourceDirectory => (GohResourceDirectory)ResourceElement;

        public MaterialLoadTreeDirectoryItemViewModel(GohResourceDirectory resourceDirectory, TreeViewModel modelsTree) : base(resourceDirectory, modelsTree) {
            Icon = s_icon;
            PropertyChangeHandler.AddHandler(nameof(IsExpanded), ExpendedHandler);
        }

        private void ExpendedHandler(object? sender, PropertyChangedEventArgs e) {
            if (!Items.Any()) {
                LoadData();
            }
        }

        private void LoadData() {
            foreach (var item in ResourceDirectory.GetDirectories()) {
                AddItem(new MaterialLoadTreeDirectoryItemViewModel(item, Tree));
            }

            foreach (var item in ResourceDirectory.Items.OfType<MaterialFile>()) {
                AddItem(new MaterialLoadTreeMaterialItemViewModel(item, Tree));
            }
        }
    }
}
