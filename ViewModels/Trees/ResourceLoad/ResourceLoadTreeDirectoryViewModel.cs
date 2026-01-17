using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public class ResourceLoadTreeDirectoryViewModel : ResourceLoadTreeItemViewModel {
        public GohResourceDirectory ResourceDirectory => (GohResourceDirectory)ResourceElement;

        public bool IsSkipEmpty { get; } = false;

        public ResourceLoadTreeDirectoryViewModel(GohResourceDirectory gohResourceDirectory, TreeViewModel modelsTree) : base(gohResourceDirectory, modelsTree) {
            Icon = IconResources.Instance.GetIcon(nameof(Resources.DirectoryIcon));

            if (GohResourceLoading.TryGetNextCompletedDirectory(gohResourceDirectory, out var nextDirectory, out string? path)) {
                Text = path;
                ResourceElement = nextDirectory;
                IsSkipEmpty = true;
            }

            if (string.IsNullOrEmpty(Text)) {
                Text = GohResourceLoading.DIRECTORY_SEPARATE.ToString();
            }

            ResourceDirectory.Update += ResourceDirectoryUpdateHandler;
        }

        private void ResourceDirectoryUpdateHandler(object? sender, EventArgs e) {
            _items.Clear();

            LoadData();
        }

        public void LoadData() {
            if (Items.Any()) {
                return;
            }

            foreach (var directory in ResourceDirectory.GetDirectories()) {
                AddItem(new ResourceLoadTreeDirectoryViewModel(directory, Tree));
            }

            var files = ResourceDirectory.GetFiles();

            if (Tree.Filter != null) {
                files = files.Where(Tree.Filter);
            }

            foreach (var mdlFiles in files) {
                AddItem(new ResourceLoadTreeFileViewModel(mdlFiles, Tree));
            }
        }

        protected override void Approve() {
            if (!Items.Any()) {
                LoadData();
            }
        }
    }
}
