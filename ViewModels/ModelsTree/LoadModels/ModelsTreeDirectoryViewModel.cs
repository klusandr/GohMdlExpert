using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GohMdlExpert.Extensions;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public class ModelsTreeDirectoryViewModel : ModelsLoadTreeItemViewModel {
        private static ImageSource s_iconSource = new BitmapImage().FromByteArray(Resources.DirectoryIcon);

        private readonly GohResourceDirectory _directory;

        private static IEnumerable<string> FileFilters { get; } = [
            @"^(?!.*_lod\d*\.)",
            @"^(?!.*#)",
        ];

        public override ICommand? DoubleClickCommand => CommandManager.GetCommand(LoadData);

        public ModelsTreeDirectoryViewModel(GohResourceDirectory directory, ModelsLoadTreeViewModel modelsTree) : base(directory, modelsTree) {
            _directory = directory;
            IconSource = s_iconSource;
        }

        public override void LoadData() {
            if (Items.Count != 0) {
                return;
            }

            var directories = _directory.GetDirectories();
            var files = _directory.GetFiles().OfType<PlyFile>().Where(f => FileFilters.All(ff => Regex.IsMatch(f.Name, ff)));

            foreach (var directory in directories) {
                AddNextNode(new ModelsTreeDirectoryViewModel(directory, Tree));
            }

            foreach (var plyFile in files) {
                AddNextNode(new ModelsTreePlyFileViewModel(plyFile, Tree));
            }
        }
    }
}
