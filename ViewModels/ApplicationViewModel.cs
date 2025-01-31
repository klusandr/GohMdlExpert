using System.IO;
using System.Timers;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell;
using GohMdlExpert.Models.GatesOfHell.Caches;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Resources.Humanskins;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using GohMdlExpert.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using WpfMvvm.ViewModels;
using WpfMvvm.ViewModels.Commands;

namespace GohMdlExpert.ViewModels {
    public class ApplicationViewModel : BaseViewModel {
        private readonly GohResourceProvider _gohResourceProvider;
        private readonly GohHumanskinResourceProvider _gohHumanskinResourceProvider;
        private readonly HumanskinMdlOverviewViewModel _models3DView;

        public float CompletionPercentage { get; set; }

        public ICommand OpenResourceCommand => CommandManager.GetCommand(OpenResourceDirectory);
        public ICommand OpenFileCommand => CommandManager.GetCommand(OpenFile);
        public ICommand CacheCommand => CommandManager.GetCommand(LoadCaches);

        public ApplicationViewModel(GohResourceProvider gohResourceProvider, GohHumanskinResourceProvider gohHumanskinResourceProvider, HumanskinMdlOverviewViewModel models3DView) {
            _gohResourceProvider = gohResourceProvider;
            _models3DView = models3DView;
            _gohHumanskinResourceProvider = gohHumanskinResourceProvider;
        }

        public void OpenFile() {
            var fileDialog = new OpenFileDialog {
                Filter = GohResourceLoading.MdlFileOpenFilter,
                InitialDirectory = Path.GetDirectoryName(Settings.Default.LastOpenedFile)
            };

            if (fileDialog.ShowDialog() ?? false) {
                Settings.Default.LastOpenedFile = fileDialog.FileName;
                _models3DView.SetMtlFile(new MdlFile(fileDialog.FileName));
            }
        }

        public void OpenResourceDirectory() {
            var folderDialog = new OpenFolderDialog() {
                InitialDirectory = Settings.Default.LastOpenedResource
            };

            if (folderDialog.ShowDialog() ?? false) {
                Settings.Default.LastOpenedResource = folderDialog.FolderName;
                _gohResourceProvider.OpenResources(folderDialog.FolderName);
            }
        }

        public void LoadCaches() {
            float completionPercentage = 0;

            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (_, _) => {
                CompletionPercentage = completionPercentage;
                OnPropertyChanged(nameof(CompletionPercentage));
            };

            timer.AutoReset = true;
            timer.Enabled = true;

            Task.Factory.StartNew(() => {
                GohCachesFilling.FillPlyTexturesCache(_gohResourceProvider, _gohHumanskinResourceProvider, ref completionPercentage);
                completionPercentage = 0;
            }).ContinueWith((t) => timer.Dispose());

            //var d =  GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>().PlyMtlsCache;
        }
    }
}
