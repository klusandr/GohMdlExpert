using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.Trees.LoadModels {
    public class ModelsLoadTreeDirectoryViewModel : ModelsLoadTreeItemViewModel {
        private static ImageSource s_icon = new BitmapImage().FromByteArray(Resources.DirectoryIcon);

        private readonly GohResourceDirectory _directory;

        public ModelsLoadTreeDirectoryViewModel(GohResourceDirectory directory, ModelsLoadTreeViewModel modelsTree) : base(directory, modelsTree) {
            _directory = directory;
            Icon = s_icon;
        }

        public override void LoadData() {
            if (Items.Any()) {
                return;
            }

            var directories = _directory.GetDirectories();
            var files = GohResourceLoading.FilterPlyFiles(_directory.GetFiles().OfType<PlyFile>());

            foreach (var directory in directories) {
                AddItem(new ModelsLoadTreeDirectoryViewModel(directory, Tree));
            }

            foreach (var plyFile in files) {
                AddItem(new ModelsLoadTreePlyFileViewModel(plyFile, Tree));
            }
        }

        public override void Approve() {
            LoadData();
        }
    }
}
