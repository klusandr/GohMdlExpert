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
        private readonly ModelsTreeViewModel _tree;
        protected string _headerText;
        protected ImageSource? _iconSource;
        protected string? _tooltip;
        private ModelsTreeItemViewModel? _parent;
        private bool _isExpended;

        public ModelsTreeViewModel Tree => _tree;

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

        public ModelsTreeItemViewModel? Parent {
            get => _parent;
            set {
                if (_parent != null) {
                    throw new InvalidOperationException($"Attempt to set {nameof(ModelsTreeItemViewModel)} parent again.");
                }

                _parent = value;
            }
        }

        public bool IsExpended {
            get => _isExpended;
            set {
                _isExpended = value;
                OnPropertyChanged();
            }
        }

        public virtual ICommand? DoubleClickCommand { get; }
        public virtual ICommand? MouseLeftClickCommand { get; }
        
        public ModelsTreeItemViewModel(ModelsTreeViewModel modelsTree) {
            _headerText = "";
            Items = [];
            _tree = modelsTree;
        }

        public virtual void AddNextNode(ModelsTreeItemViewModel viewModel) {
            viewModel.Parent ??= this;
            Items.Add(viewModel);
        }
    }
}
