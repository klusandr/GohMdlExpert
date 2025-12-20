using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.Trees.LoadModels {
    public class ModelsLoadTreePlyLodFilesViewModel : ModelsLoadTreeItemViewModel {
        public ModelsLoadTreePlyLodFilesViewModel(IEnumerable<PlyFile> lodPlyFiles, ModelsLoadTreeViewModel modelsTree) : base(GetDirectory(lodPlyFiles), modelsTree) {
            ContextMenuViewModel.ClearItems();

            Icon = IconResources.Instance.GetIcon(nameof(Resources.DirectoryIcon));

            int lodIndex = 1;
            foreach (var file in lodPlyFiles) {
                AddItem(new ModelsLoadTreePlyLodFileViewModel(file, lodIndex++, modelsTree));
            }

            IsExpanded = true;
        }

        public override void LoadData() { }

        private static GohResourceVirtualDirectory GetDirectory(IEnumerable<PlyFile> lodPlyFiles) {
            var directory = new GohResourceVirtualDirectory("LOD");

            directory.Items.AddRange(lodPlyFiles);

            return directory;
        }
    }
}
