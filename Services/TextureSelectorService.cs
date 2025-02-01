using GohMdlExpert.Models.GatesOfHell.Resources.Data;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Views;

namespace GohMdlExpert.Services {
    public class TextureSelectorService {
        private TextureOverviewView? _materialLoad;
        private ChildWindow? _childWindow;

        public MtlTexture? SelectedTexture { get; set; }

        public event EventHandler? SelectedTextureChange;

        public TextureSelectorService() { }

        public MtlTexture? GetMaterialDialog() {
            _materialLoad ??= LoadView();

            _materialLoad.ViewModel.Texture = SelectedTexture;

            _childWindow = new ChildWindow() {
                Title = $"Load materials",
                Content = _materialLoad,
            };

            _childWindow.ShowDialog();

            return SelectedTexture;
        }

        private TextureOverviewView LoadView() {
            var view = new TextureOverviewView();

            view.ViewModel.TextureApprove += MaterialApproveHandler;
            view.ViewModel.TextureApply += MaterialApplyHandler;

            return view;
        }

        private void MaterialApplyHandler(object? sender, EventArgs e) {
            SelectedTexture = _materialLoad?.ViewModel.Texture;
            SelectedTextureChange?.Invoke(this, EventArgs.Empty);
        }

        private void MaterialApproveHandler(object? sender, EventArgs e) {
            SelectedTexture = _materialLoad?.ViewModel.Texture;
            _childWindow?.Close();
        }
    }
}
