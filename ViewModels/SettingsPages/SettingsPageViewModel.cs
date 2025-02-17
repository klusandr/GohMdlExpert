using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public abstract class SettingsPageViewModel : BaseViewModel {
        public abstract string Name { get; }
    }
}
