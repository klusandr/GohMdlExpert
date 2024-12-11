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

namespace GohMdlExpert.ViewModels.ModelsTree.LoadModels {
    public abstract class ModelsLoadTreeItemViewModel : ModelsTreeItemViewModel {
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

        public virtual ICommand LoadCommand => CommandManager.GetCommand(() => { });
        public virtual ICommand DeleteCommand => CommandManager.GetCommand(() => { });

        public ModelsLoadTreeItemViewModel(GohResourceElement resourceElement, ModelsLoadTreeViewModel modelsTree) : base(modelsTree) {
            _isButtonActive = false;

            HeaderText = resourceElement.Name;
            ToolTip = resourceElement.GetFullPath();
            ResourceElement = resourceElement;
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
    }
}
