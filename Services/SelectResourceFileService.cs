using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.ViewModels.Dialogs;
using GohMdlExpert.Views.Dialogs;

namespace GohMdlExpert.Services {
    public class SelectResourceFileService {
        private readonly SelectResourceFileDialogView _dialogView;

        private SelectResourceFileDialogViewModel _dialogViewModel => _dialogView.ViewModel;

        public SelectResourceFileService() {
            _dialogView = new SelectResourceFileDialogView();
        }

        public T? SelectResourceFile<T>(Func<GohResourceFile, bool>? filter = null, string? initPath = null) where T : GohResourceFile {
            var fileType = typeof(T);

            var childWindow = new ChildWindow() {
                Title = "Select resource",
                Content = _dialogView,
            };

            _dialogViewModel.SelectEvent += (_, _) => {
                childWindow.Close();
            };

            _dialogViewModel.CancelEvent += (_, _) => {
                childWindow.Close();
            };

            _dialogViewModel.Clear();
            _dialogViewModel.Tree.Filter = filter;

            if (_dialogView.ViewModel.ResourceFileType != fileType) {
                _dialogViewModel.ResourceFileType = fileType;
                _dialogViewModel.Update();
            }

            if (initPath != null) {
                _dialogViewModel.Tree.ExpandToResourceElement(initPath);
            }

            childWindow.ShowDialog();

            return (T?)_dialogViewModel.SelectedResource;
        }
    }
}
