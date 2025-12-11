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

        public Statuses Status {
            get => _status;
            set {
                _status = value;
                OnPropertyChanged();
            }
        }

        public ResourceSaveListItemViewModel(GohResourceElement resourceElement) : base(resourceElement) { }
    }
}
