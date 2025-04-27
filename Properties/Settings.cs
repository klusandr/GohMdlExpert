using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvm.Views;

namespace GohMdlExpert.Properties {
    public class Settings : WpfMvvm.Properties.Settings {
        private static Settings? s_default;
        
        public static Settings Default => s_default ??= new Settings();

        [UserScopedSetting()]
        [DefaultValue(null)]
        public string? LastOpenedResource {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [UserScopedSetting()]
        [DefaultValue(null)]
        public string? LastOpenedFile {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [UserScopedSetting()]
        [DefaultValue(null)]
        public string? LastSavedFile {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [UserScopedSetting()]
        [DefaultValue(null)]
        public string? GameDirectoryPath {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [UserScopedSetting()]
        [DefaultValue(null)]
        public string? GameVersion {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [UserScopedSetting()]
        [DefaultValue(null)]
        public string? ThemeName {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [UserScopedSetting()]
        [DefaultValue(true)]
        public bool LoadGameResourceOnStart {
            get => GetValue<bool>();
            set => SetValue(value);
        }
    }
}
