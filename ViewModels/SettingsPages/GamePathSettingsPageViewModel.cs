using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Properties;
using Microsoft.Win32;

namespace GohMdlExpert.ViewModels.SettingsPages {
    public class GamePathSettingsPageViewModel : SettingsPageViewModel {
        private string? _gameDirectoryPath;
        private readonly GohGameDirectory _gameDirectory;

        public override string Name { get; } = nameof(GamePathSettingsPageViewModel);

        public string? GameDirectoryPath => _gameDirectory.Path;
        public Version? Version => _gameDirectory.Version;

        public ICommand ReviewPathCommand => CommandManager.GetCommand(ReviewPath);

        public GamePathSettingsPageViewModel(GohGameDirectory gameDirectory) {
            _gameDirectory = gameDirectory;
            gameDirectory.Updated += (_, _) => UpdateData();
        }

        public void ReviewPath() {
            var folderDialog = new OpenFolderDialog() {
                InitialDirectory = GameDirectoryPath
            };

            if (folderDialog.ShowDialog() ?? false) {
                _gameDirectory.Open(folderDialog.FolderName);

                Settings.Default.GameDirectoryPath = _gameDirectory.Path;
                Settings.Default.GameVersion = _gameDirectory.Version?.ToString();

                UpdateData();
            }
        }

        private void UpdateData() {
            OnPropertyChanged(nameof(GameDirectoryPath));
            OnPropertyChanged(nameof(Version));
        }
    }
}
