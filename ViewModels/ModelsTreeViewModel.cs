using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using MvvmWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GohMdlExpert.ViewModels {
    public class ModelsTreeViewModel : ViewModelBase {
        private Models3DViewModel? _models3DViewModel;

        public ObservableCollection<TreeViewItem> ModelsFiles { get; set; }

        public ICommand LoadModelsCommand => CommandManager.GetCommand(LoadModels);

        public Models3DViewModel Models3DViewModel => _models3DViewModel ??= ViewModelManager.GetViewModel<Models3DViewModel>()!;

        public ModelsTreeViewModel() {
            ModelsFiles = new ();
        }

        public void LoadModels() {
            var d = GohResourceLoader.Instance.GetResourceDirectory("ger_humanskin_source");

            var item = new TreeViewItem() { Header = "[get_source]" };
            ModelsFiles.Add(item);

            LoadPlyDirectory(d, item);

        }

        public void LoadPlyDirectory(GohResourceDirectory currentDirectory, TreeViewItem currentItem) {
            var directories = currentDirectory.GetDirectories();
            var files = currentDirectory.GetFiles().OfType<PlyFile>().Where(f => !f.Name.Contains("lod"));

            foreach (var directory in directories) {
                var newItem = new TreeViewItem() { Header = directory.Name };
                currentItem.Items.Add(newItem);
                LoadPlyDirectory(directory, newItem);
            }

            foreach (var file in files) {
                var newItem = new TreeViewItem() {
                    Header = file.Name,
                    DataContext = file,
                };
                currentItem.Items.Add(newItem);
                newItem.MouseDoubleClick += PlyModelMouseDoubleClick;
            }
        }

        private void PlyModelMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is TreeViewItem item && item.IsSelected) {
                var plyFile = (PlyFile)item.DataContext;

                var plyModel = plyFile.Data;
                var textures = GohResourceLoader.Instance.GetPlyMtlFiles(plyFile);

                foreach (var mesh in plyModel.Meshes!) {
                    var newItem = new TreeViewItem() {
                        Header = mesh.TextureFileName,
                        DataContext = mesh,
                    };

                    var defTextures = textures
                        .Where(t => mesh.TextureFileName.Contains(t.Name))
                        .Select(t => t.Data.Diffuse)
                        .Distinct();

                    foreach (var texture in defTextures) {
                        newItem.Items.Add(new TreeViewItem() {
                            Header = texture.Name,
                            ToolTip = texture.GetFullPath(),
                            DataContext = texture,
                        });
                    }

                    item.Items.Add(newItem);
                }
            }
        }
    }
}
