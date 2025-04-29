﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using Microsoft.Win32;
using WpfMvvm.Collections.ObjectModel;
using GohMdlExpert.Properties;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public class ModsSettingsPageViewModel : SettingsPageViewModel {
        public class ModViewModel {
            private readonly ModResource _modResource;

            public string? Name => _modResource.ModInfo?.Name;
            public string Path => _modResource.Path;
            public bool IsEnable {
                get => _modResource.IsEnable;
                set => _modResource.IsEnable = value;
            }
            public bool IsLoad => _modResource.IsLoad;

            public ModResource ModResource => _modResource;

            public ModViewModel(ModResource modResource) {
                _modResource = modResource;
            }
        }

        private readonly GohResourceProvider _resourceProvider;
        private readonly GohModResourceProvider _modResourceProvider;
        private readonly ObservableList<ModViewModel> _mods;
        private ModViewModel? _selectedMod;

        public override string Name => "Mod manager";

        public IObservableEnumerable<ModViewModel> Mods => _mods;

        public ModViewModel? SelectedMod {
            get => _selectedMod;
            set {
                _selectedMod = value;
                CommandManager.OnCommandCanExecuteChanged(nameof(RemoveModCommand));
                OnPropertyChanged();
            }
        }

        public ICommand AddModCommand => CommandManager.GetCommand(AddMod);
        public ICommand RemoveModCommand => CommandManager.GetCommand(RemoveMod, canExecute: (_) => SelectedMod != null);
        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);
        

        public ModsSettingsPageViewModel(GohResourceProvider resourceProvider, GohModResourceProvider modResourceProvider) {
            _resourceProvider = resourceProvider;
            _modResourceProvider = modResourceProvider;
            _mods = [];
        }

        private void AddMod() {
            var folderDialog = new OpenFolderDialog() {
                InitialDirectory = Settings.Default.LastOpenedResource
            };

            ModResource mod;

            if (folderDialog.ShowDialog() ?? false) {
                Settings.Default.LastOpenedResource = folderDialog.FolderName;
                mod = new ModResource(folderDialog.FolderName);
                _modResourceProvider.AddMod(mod);
                _mods.Add(new ModViewModel(mod));
            }
        }

        private void RemoveMod() {
            if (SelectedMod != null) {
                _modResourceProvider.RemoveMod(SelectedMod.ModResource);
                _mods.Remove(SelectedMod);
            }
        }

        private void Approve() {
            _resourceProvider.LoadModResources();
        }
    }
}
