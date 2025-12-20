using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels.Controls;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.Trees.Humanskins {
    public class HumanskinTreeDirectoryViewModel : HumanskinTreeItemViewModel {
        private readonly GohResourceDirectory _directory;

        public ICommand UpdateCommand => CommandManager.GetCommand(UpdateDirectoryData);

        public HumanskinTreeDirectoryViewModel(GohResourceDirectory directory, TreeViewModel modelsTree) : base(directory, modelsTree) {
            _directory = directory;

            ContextMenuViewModel.AddItem(new MenuItemViewModel("Update", UpdateCommand));

            directory.Update += DirectoryUpdateHandler;

            Icon = IconResources.Instance.GetIcon(nameof(Resources.DirectoryIcon));
        }

        public void LoadData() {
            if (Items.Any()) {
                return;
            }

            foreach (var directory in _directory.GetDirectories()) {
                AddItem(new HumanskinTreeDirectoryViewModel(directory, Tree));
            }

            foreach (var mdlFiles in _directory.GetFiles().OfType<MdlFile>()) {
                AddItem(new HumanskinTreeHumanskinViewModel(mdlFiles, Tree));
            }
        }

        public void UpdateData() {
            _items.Clear();
            LoadData();
        }

        public void UpdateDirectoryData() {
            _directory.UpdateData();
        }

        protected override void Approve() {
            LoadData();
        }

        private void DirectoryUpdateHandler(object? sender, EventArgs e) {
            UpdateData();
        }
    }
}
