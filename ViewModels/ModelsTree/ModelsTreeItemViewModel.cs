using GohMdlExpert.Views.ModelsTree;
using MvvmWpf.ViewModels;
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
    public abstract class ModelsTreeItemViewModel : ViewModelBase {
        private string _headerText;
        private ImageSource? _iconSource;
        private string? _tooltip;
        private bool _approved;

        protected ModelsTreeViewModel ModelsTree { get; }

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
            set {
                _approved = value;
                OnPropertyChanged();
            }
        }

        public virtual ICommand? DoubleClickCommand { get; }
        public virtual ICommand? ExpandedCommand { get; }


        public ModelsTreeItemViewModel(ModelsTreeViewModel modelsTree, ModelsTreeItemViewModel? parent = null) {
            _headerText = "";
            Items = [];
            ModelsTree = modelsTree;
            Parent = parent;
        }

        public void AddNextNode(ModelsTreeItemViewModel viewModel) {
            Items.Add(viewModel);
        }
    }
}
