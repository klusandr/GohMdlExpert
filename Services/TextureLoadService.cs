using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views;

namespace GohMdlExpert.Services {
    public class TextureLoadService {
        private TextureLoadView? _materialLoad;
        private ChildWindow? _childWindow;

        public MtlTexture? SelectedTexture { get; set; }

        public event EventHandler? SelectedTextureChange;

        public TextureLoadService() { }

        public MtlTexture? ShowDialog() {
            _materialLoad ??= LoadView();

            if (SelectedTexture != null) {
                _materialLoad.ViewModel.Texture = SelectedTexture;
            }

            _childWindow = new ChildWindow() {
                Title = $"Load materials",
                Content = _materialLoad,
            };

            _childWindow.ShowDialog();

            var texture = GetSelectedTexture();

            ClearSelect();

            return texture;
        }

        private TextureLoadView LoadView() {
            var view = new TextureLoadView();

            view.ViewModel.CancelEvent += CloseWibdow;
            view.ViewModel.ApplyEvent += ApplyEventHandler;
            view.ViewModel.OkEvent += CloseWibdow;

            return view;
        }

        private MtlTexture? GetSelectedTexture() {
            var viewModel = _materialLoad?.ViewModel;

            if (viewModel?.Texture.Diffuse is not NullMaterialFile) {
                return viewModel?.Texture;
            } else {
                return null;
            }
        }

        private void ClearSelect() {
            _materialLoad?.ViewModel.ClearTexture();
        }

        private void ApplyEventHandler(object? sender, EventArgs e) {
            SelectedTexture = GetSelectedTexture();
            SelectedTextureChange?.Invoke(this, EventArgs.Empty);
        }

        private void CloseWibdow(object? sender, EventArgs e) {
            _childWindow?.Close();
        }
    }
}
