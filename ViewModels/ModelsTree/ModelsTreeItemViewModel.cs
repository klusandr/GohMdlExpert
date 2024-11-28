using GohMdlExpert.Views.ModelsTree;
using WpfMvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GohMdlExpert.ViewModels.ModelsTree {
    public class ModelsTreeItemViewModel : BaseViewModel {
        private string _headerText;
        private ImageSource? _iconSource;
        private string? _tooltip;
        private bool _approved;

        public ModelsTreeViewModel Tree { get; }

        public ModelsTreeItemViewModel? Parent { get; }
        public ObservableCollection<ModelsTreeItemViewModel> Items { get; }

        public string HeaderText {
            get => _headerText;
            set {
                _headerText = value;
                OnPropertyChanged();
            }
        }

        public ImageSource? IconSource {
            get => _iconSource;
            set {
                _iconSource = value;
                OnPropertyChanged();
            }
        }

        public string? ToolTip {
            get => _tooltip;
            set {
                _tooltip = value;
                OnPropertyChanged();
            }
        }

        public bool IsApproved {
            get => _approved;
            private set {
                _approved = value;
                OnPropertyChanged();
            }
        }

        public virtual ICommand? DoubleClickCommand { get; }

        public ModelsTreeItemViewModel(ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) {
            _headerText = "";
            Items = [];
            Tree = modelsTree;
            Parent = parent;
        }

        public void AddNextNode(ModelsTreeItemViewModel viewModel) {
            Items.Add(viewModel);
        }

        public void Approve() {
            Tree.ApproveItem(this);
            IsApproved = true;
        }

        public void CancelApprove() {
            IsApproved &= false;
        }
    }
}
