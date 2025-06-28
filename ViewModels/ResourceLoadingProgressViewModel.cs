using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using SeoMeterVerifier.ViewModels;
using WpfMvvm.ViewModels;
using Timer = System.Timers.Timer;

namespace GohMdlExpert.ViewModels {
    public class ResourceLoadingProgressViewModel : BaseViewModel {
        private const int UPDATE_INTERVAL = 100;

        private readonly string[] _stepsText = ["Load humanskins", "Load textures"];
        private readonly GohResourceDirectory _textureDirectoty;
        private readonly IEnumerable<GohResourceDirectory> _progressDirectories;
        private int _filesCount;
        private int _directoriesCount;
        private string _stepText;
        private Timer _updateTimer;
        private string? _currentDirectoryFullName;
        private bool _isEnd;

        public ProgressBarViewModel ProgressBar { get; }
        public int FilesCount => _filesCount;
        public int DirectoriesCount => _directoriesCount;

        public string CurrentStepText {
            get => _stepText;
            set {
                _stepText = value;
                OnPropertyChanged();
            }
        }

        public string? CurrentDirectoryFullName {
            get => _currentDirectoryFullName;
            set {
                _currentDirectoryFullName = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnd {
            get => _isEnd;
            set {
                _isEnd = value;
                OnPropertyChanged();
            }
        }

        public ResourceLoadingProgressViewModel(GohResourceProvider resourceProvider) {
            ProgressBar = new ProgressBarViewModel();
            _textureDirectoty = resourceProvider.GetLocationDirectory(GohResourceLocations.Texture);
            _progressDirectories = (IEnumerable<GohResourceDirectory>)[
                .. resourceProvider.GetLocationDirectory(GohResourceLocations.Humanskin).GetDirectories(),
                .. resourceProvider.GetLocationDirectory(GohResourceLocations.Texture).GetDirectories(),
            ];

            ProgressBar.Step = ProgressBar.Maximum / _progressDirectories.Count() + 1;

            _stepText = _stepsText[0];

            _updateTimer = new Timer(UPDATE_INTERVAL);
            _updateTimer.Elapsed += UpdateTimerElapsedHandler;

            _updateTimer.Start();
        }

        public void LoadElementHandler(GohResourceElement element) {
            if (element is GohResourceDirectory directory) {
                if (directory == _textureDirectoty) {
                    NextSteap();
                } else if (_progressDirectories.Contains(directory)) {
                    Progress();
                }

                CurrentDirectoryFullName = directory.GetFullPath();
                _directoriesCount++;
            } else {
                _filesCount++;
            }
        }

        public void EndLoadingHandler() {
            _updateTimer.AutoReset = false;
            IsEnd = true;
        }

        private void NextSteap() {
            CurrentStepText = _stepsText[1];
        }

        private void Progress() {
            ProgressBar.AddStep();
        }

        private void UpdateTimerElapsedHandler(object? sender, System.Timers.ElapsedEventArgs e) {
            OnPropertyChanged(nameof(FilesCount));
            OnPropertyChanged(nameof(DirectoriesCount));
        }
    }
}
