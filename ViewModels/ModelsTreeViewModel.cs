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
            var d = ResourceLocations.Instance.GetLocationPath("ger_humanskin_source");

            var item = new TreeViewItem() { Header = "[get_source]" };
            ModelsFiles.Add(item);

            LoadPlyDirectory(d, item);

        }

        public void LoadPlyDirectory(string path, TreeViewItem currentItem) {
            var directories = Directory.GetDirectories(path);
            var fileNames = Directory.GetFiles(path).Where(f => Path.GetExtension(f) == ".ply" && !f.Contains("lod"));
            
            foreach (var directory in directories) {
                var newItem = new TreeViewItem() { Header = GetLastDirectory(directory) };
                currentItem.Items.Add(newItem);
                LoadPlyDirectory(directory, newItem);
            }

            foreach (var file in fileNames) {
                var newItem = new TreeViewItem() { 
                    Header = Path.GetFileName(file),
                    DataContext = file,
                };
                currentItem.Items.Add(newItem);
                newItem.MouseDoubleClick += PlyModelMouseDoubleClick;
            }
        }

        private void PlyModelMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is TreeViewItem item) {
                Models3DViewModel.OpenPlyFile((string)item.DataContext);
            }
        }

        private string GetLastDirectory(string path) {
            return path[(path.LastIndexOf("\\") + 1)..];
        }
    }
}
