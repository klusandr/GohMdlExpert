using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public class ResourceLoadTreeViewModel : TreeViewModel {
        public GohResourceDirectory? Root { get; set; }
        public Func<GohResourceFile, bool>? Filter { get; internal set; }

        public new ResourceLoadTreeItemViewModel? SelectedItem { 
            get => (ResourceLoadTreeItemViewModel?)base.SelectedItem; 
            set => base.SelectedItem = value; 
        }

        public ResourceLoadTreeViewModel() { }

        public override void LoadData() {
            ClearData();

            if (Root != null) {
                AddItem(new ResourceLoadTreeDirectoryViewModel(Root, this));
            }
        }

        public void ExpandetDirectory(string path) {
            if (Root != null) {
                var directory = Root.AlongPath(path);

                if (directory != null) {
                    var directoryItem = FindItem((item) => {
                        if (item is ResourceLoadTreeDirectoryViewModel directoryItem) {
                            if (directoryItem.ResourceDirectory.GetFullPath() == directory.GetFullPath()) {
                                return true;
                            } else if (!item.Items.Any()) {
                                directoryItem.LoadData();
                            }

                            return false;
                        }

                        return false;
                    });

                    if (directoryItem != null) {
                        ((ResourceLoadTreeDirectoryViewModel)directoryItem).LoadData();
                        directoryItem.IsSelected = true;
                        directoryItem.IsExpanded = true;
                        ExpendToItem(directoryItem);
                    }
                }
            }
        }
    }
}
