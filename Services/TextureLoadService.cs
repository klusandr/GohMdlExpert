using System.Windows.Media.Media3D;
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

        public MtlTexture? GetMaterialDialog() {
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

            view.ViewModel.TextureApprove += MaterialApproveHandler;
            view.ViewModel.TextureApply += MaterialApplyHandler;

            return view;
        }

        private MtlTexture? GetSelectedTexture() {
            if (_materialLoad?.ViewModel.Texture.Diffuse is not NullMaterialFile) {
                return _materialLoad?.ViewModel.Texture;
            } else {
                return null;
            }
        }

        private void ClearSelect() {
            _materialLoad?.ViewModel.ClearTexture();
        }

        private void MaterialApplyHandler(object? sender, EventArgs e) {
            SelectedTexture = GetSelectedTexture();
            SelectedTextureChange?.Invoke(this, EventArgs.Empty);
        }

        private void MaterialApproveHandler(object? sender, EventArgs e) {
            _childWindow?.Close();
        }
    }
}
