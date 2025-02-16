using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Textures {
    public class TextureLoadTreeDirectoryViewModel : TextureLoadTreeItemViewModel {
        private class MtlFileComparer : IComparer<MtlFile> {
            public int Compare(MtlFile? x, MtlFile? y) {
                return Comparer<string>.Default.Compare(x?.Name, y?.Name) + Comparer<string>.Default.Compare(x?.Data.Diffuse.Name, y?.Data.Diffuse.Name);
            }
        }

        private static MtlFileComparer s_mtlFileComparer = new MtlFileComparer();

        private readonly IEnumerable<string> _mtlFilesNames;

        public TextureLoadTreeDirectoryViewModel(string name, IEnumerable<string> mtlFilesNames, TreeViewModel modelsTree) : base(modelsTree) {
            Text = name;
            Icon = IconResources.Instance.GetIcon(nameof(Resources.DirectoryIcon));

            PropertyChangeHandler.AddHandler(nameof(IsExpanded), ExpandedChangeHandler);
            _mtlFilesNames = mtlFilesNames;
        }

        private void ExpandedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsExpanded) {
                LoadData(_mtlFilesNames);
            }
        }

        private void LoadData(IEnumerable<string> mtlFilesNames) {
            var mtlFiles = new List<MtlFile>();

            foreach (var mtlFileName in mtlFilesNames) {
                var file = Tree.ResourceProvider.GetFile(mtlFileName);

                if (file is MtlFile mtlFile) {
                    mtlFiles.Add(mtlFile);
                }
            }

            mtlFiles.Sort(s_mtlFileComparer);

            foreach (var mtlFile in mtlFiles) {
                Tree.TextureProvider.SetTextureMaterialsFullPath(mtlFile.Data);
                AddItem(new TextureLoadTreeTextureViewModel(mtlFile, Tree));
            }
        }
    }
}
