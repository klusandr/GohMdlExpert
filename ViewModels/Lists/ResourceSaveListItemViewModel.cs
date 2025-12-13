using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.Data;

namespace GohMdlExpert.ViewModels.Lists {
    public class ResourceSaveListItemViewModel : ResourceLoadListItemViewModel {
        private Statuses _status;
        private string? _message;
        private string? _saveAction;

        public Statuses Status {
            get => _status;
            set {
                _status = value;
                UpdateSaveAction();
                OnPropertyChanged();
            }
        }

        public string? Message {
            get => _message;
            set {
                _message = value;
                ToolTip = value;
                
                OnPropertyChanged();
            }
        }

        public string? SaveAction {
            get => _saveAction;
            set {
                _saveAction = value;
                OnPropertyChanged();
            }
        }

        public ResourceSaveListItemViewModel(GohResourceElement resourceElement) : base(resourceElement) { }

        public void UpdateSaveAction() {
            SaveAction = Status switch {
                Statuses.None => null,
                Statuses.Good => "Create",
                Statuses.Warning => "Replace",
                Statuses.Error => "Replace. May be errors.",
                _ => null,
            };
        }
    }
}
