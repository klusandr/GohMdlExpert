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

        private readonly GohResourceDirectory _resourceDirectory;

        public TextureLoadTreeDirectoryViewModel(GohResourceDirectory resourceDirectory, TreeViewModel modelsTree) : base(modelsTree) {
            Text = resourceDirectory.Name;
            Icon = IconResources.Instance.GetIcon(nameof(Resources.DirectoryIcon));

            PropertyChangeHandler.AddHandler(nameof(IsExpanded), ExpandedChangeHandler);
            _resourceDirectory = resourceDirectory;
        }

        private void ExpandedChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (IsExpanded) {
                if (_items.Count == 0) {
                    LoadData();
                }
            }
        }

        private void LoadData() {
            foreach (var mtlFile in _resourceDirectory.GetFiles().Cast<MtlFile>()) {
                AddItem(new TextureLoadTreeTextureViewModel(mtlFile, Tree));
            }
        }
    }
}
