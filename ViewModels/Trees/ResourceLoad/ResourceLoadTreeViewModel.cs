using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.ResourceLoad {
    public class ResourceLoadTreeViewModel : TreeViewModel {
        public GohResourceDirectory? Root { get; set; }
        public Func<GohResourceFile, bool>? Filter { get; set; }

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

        public void ExpandToResourceElement(GohResourceElement resourceElement) {
            ExpandToResourceElement(GetInsidePath(resourceElement.GetFullPath()));
        }

        public void ExpandToResourceElement(string path) {
            if (Root != null) {
                var resourceItem = AlongPath(path);

                if (resourceItem != null) {
                    if (resourceItem is ResourceLoadTreeDirectoryViewModel directoryItem) {
                        directoryItem.LoadData();
                        resourceItem.IsExpanded = true;
                    }

                    resourceItem.IsSelected = true;
                    ExpendToItem(resourceItem);
                }
            }
        }

        public ResourceLoadTreeItemViewModel? AlongPath(string path) {
            return AlongPath(PathUtils.GetPathComponents(path));
        }

        public ResourceLoadTreeItemViewModel? AlongPath(IEnumerable<string> pathComponents, ResourceLoadTreeItemViewModel? currentItem = null) {
            currentItem ??= (ResourceLoadTreeItemViewModel)Items.First();

            if (!pathComponents.Any()) {
                return currentItem;
            }

            string currentPathComponent = pathComponents.First();

            if (currentItem is ResourceLoadTreeDirectoryViewModel directoryItem) {
                if (directoryItem.IsSkipEmpty) {
                    var skipPathComponents = PathUtils.GetPathComponents(directoryItem.Text!);

                    for (int i = 0; i < skipPathComponents.Length; i++) {
                        if (skipPathComponents[i] != pathComponents.ElementAt(i)) {
                            return null;
                        }
                    }

                    pathComponents = pathComponents.Skip(skipPathComponents.Length);
                }

                directoryItem.LoadData();

                currentItem = directoryItem.Items.Cast<ResourceLoadTreeItemViewModel>().FirstOrDefault(i => i.ResourceElement.Name == currentPathComponent);

                if (pathComponents.Any()) {
                    return AlongPath(pathComponents.Skip(1), currentItem);
                } else {
                    return currentItem;
                }
            } else if (currentItem.ResourceElement.Name == currentPathComponent && pathComponents.Count() == 1) {
                return currentItem;
            }

            return null;
        }

        private string GetInsidePath(string path) {
            if (Root != null) {
                if (Root.Name != string.Empty) {
                    return path.Replace(Root.GetFullPath(), null);
                } else {
                    return path;
                }
            }

            return string.Empty;
        }
    }
}
