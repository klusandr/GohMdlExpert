using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using WpfMvvm.ViewModels.Controls;
using System.Diagnostics;
using System.IO;
using WpfMvvm.ViewModels.Controls.Menu;

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public abstract class ModelsLoadTreeItemViewModel : TreeItemViewModel {
        private bool _approved;
        private bool _isButtonActive;

        public new ModelsLoadTreeViewModel Tree => (ModelsLoadTreeViewModel)base.Tree;

        public GohResourceElement ResourceElement { get; }

        public bool IsApproved {
            get => _approved;
            private set {
                _approved = value;
                OnPropertyChanged();
            }
        }

        public bool IsButtonActive {
            get => _isButtonActive;
            set {
                _isButtonActive = value;
                OnPropertyChanged();
            }
        }

        public virtual ICommand ApproveCommand => CommandManager.GetCommand(Approve);
        public virtual ICommand OpenInExplorerCommand => CommandManager.GetCommand(OpenInExplorer);

        public virtual ICommand? LoadCommand => null;
        public virtual ICommand? DeleteCommand => null;

        public ModelsLoadTreeItemViewModel(GohResourceElement resourceElement, ModelsLoadTreeViewModel modelsTree) : base(modelsTree) {
            _isButtonActive = false;

            Text = resourceElement.Name;
            ToolTip = resourceElement.GetFullPath();
            ResourceElement = resourceElement;
            ContextMenuViewModel.AddItem(new MenuItemViewModel("Open in explorer", OpenInExplorerCommand));

            modelsTree.CreatedItemHandler(this, EventArgs.Empty);
        }

        public abstract void LoadData();

        public virtual void UpdateData() {
            ClearData();
            LoadData();
        }

        public virtual void ClearData() {
            Items.Clear();
        }

        public virtual void Approve() {
            if (!IsApproved) {
                IsApproved = true;
            }
        }

        public virtual void CancelApprove() {
            if (IsApproved) {
                IsApproved = false;
            }
        }

        private void OpenInExplorer() {
            #warning Вынести куда нибудб открытие файла в проводника
            Process.Start("explorer.exe", $"/select, {ResourceElement.GetFullPath()}");
        }
    }
}
