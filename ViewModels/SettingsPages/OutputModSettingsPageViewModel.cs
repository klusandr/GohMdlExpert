using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Mods;
using Microsoft.Win32;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public class OutputModSettingsPageViewModel : SettingsPageViewModel {
        private readonly GohResourceProvider _resourceProvider;
        private readonly GohOutputModProvider _modProvider;
        private readonly GohGameDirectory _gameDirectory;
        private OutputModResource? _mod;
        public readonly static string PageName = "OutputModSettings";

        public override string Name => PageName;

        public string? ModPath => _mod?.Path;

        public bool IsEnable {
            get => _mod?.IsEnable ?? false;
            set {
                if (_mod != null) {
                    _mod.IsEnable = value;
                }
                OnPropertyChanged();
            }
        }

        public bool LoadOnStart {
            get => Settings.LoadOutputModOnStart;
            set {
                Settings.LoadOutputModOnStart = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetModCommand => CommandManager.GetCommand(SetMod);

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);

        public OutputModSettingsPageViewModel(GohResourceProvider resourceProvider, GohOutputModProvider modProvider, GohGameDirectory gameDirectory) {
            _resourceProvider = resourceProvider;
            _modProvider = modProvider;
            _gameDirectory = gameDirectory;

            _modProvider.ModUpdate += ModUpdateHandler;
        }

        public override void LoadSettings() {
            string? path = Settings.OutputModPath;

            if (path != null) {
                _modProvider.Mod = new OutputModResource(path) { };
            }
        }

        public override void SaveSettings() {
            Settings.OutputModPath = _mod?.Path;

            base.SaveSettings();
        }

        private void SetMod() {
            var folderDialog = new OpenFolderDialog();

            if (_mod == null) {
                if (_gameDirectory.Path != null) {
                    folderDialog.InitialDirectory = _gameDirectory.Path;
                }
            } else {
                folderDialog.InitialDirectory = _mod.Path;
            }

            if (folderDialog.ShowDialog() ?? false) {
                _mod = new OutputModResource(folderDialog.FolderName);
            }

            OnPropertyChanged("");
        }

        private void Approve() {
            ApproveSettings();

            if (_mod != null) {
                _modProvider.Mod = _mod;
            } else {
                _modProvider.ClearMod();
            }

            _resourceProvider.LoadAllResources();
            _resourceProvider.FullLoad();
            SaveSettings();
        }

        private void ModUpdateHandler(object? sender, EventArgs e) {
            _mod = _modProvider.ModIsLoaded ? _modProvider.Mod : null;
            OnPropertyChanged("");
        }
    }
}
