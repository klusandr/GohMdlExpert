﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public abstract class SettingsPageViewModel : BaseViewModel {
        internal Settings Settings { get; } = Settings.Default;
        public abstract string Name { get; }
    }
}
