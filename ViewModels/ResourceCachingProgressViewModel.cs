using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using SeoMeterVerifier.ViewModels;
using WpfMvvm.ViewModels;
using Timer = System.Timers.Timer;

namespace GohMdlExpert.ViewModels {
    public class ResourceCachingProgressViewModel : BaseViewModel {
        private const int UPDATE_INTERVAL = 100;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private string? _currentFile;
        private int _filesCount;
        private Timer _updateTimer;
        private bool _isEnd;
        private float _procantage;

        public ProgressBarViewModel ProgressBar { get; }
        public int FilesCount => _filesCount;

        public bool IsEnd {
            get => _isEnd;
            set {
                _isEnd = value;
                OnPropertyChanged();
            }
        }

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public ICommand CancelCommand => CommandManager.GetCommand(Cancel);

        public string? CurrentFile { get => _currentFile; }

        public event EventHandler? Canceled;

        public ResourceCachingProgressViewModel() {
            ProgressBar = new ProgressBarViewModel();

            _cancellationTokenSource = new CancellationTokenSource();

            _updateTimer = new Timer(UPDATE_INTERVAL);
            _updateTimer.Elapsed += UpdateTimerElapsedHandler;

            _updateTimer.Start();
        }

        public void LoadFileHandler(string currentFileName, float procantage) {
            _currentFile = currentFileName;
            _filesCount++;
            _procantage = procantage;
        }

        public void EndLoadingHandler() {
            _updateTimer.AutoReset = false;
            IsEnd = true;
        }

        private void Cancel() {
            _cancellationTokenSource.Cancel();
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateTimerElapsedHandler(object? sender, System.Timers.ElapsedEventArgs e) {
            OnPropertyChanged(nameof(FilesCount));
            OnPropertyChanged(nameof(CurrentFile));
            ProgressBar.Value = (int)_procantage;
        }
    }
}
