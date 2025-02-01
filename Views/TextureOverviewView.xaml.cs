using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GohMdlExpert.ViewModels;
using WpfMvvm.Collections;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для MaterialLoadView.xaml
    /// </summary>
    [BindingViewModel<TextureLoadViewModel>]
    [BindingViewModelViaDI]
    public partial class TextureOverviewView : BaseView {
        private readonly IEnumerable<TextBox> _selectedTextBlokGroup;
        private readonly Style _selectTextBoxStyle;

        public new TextureLoadViewModel ViewModel => (TextureLoadViewModel)base.ViewModel;

        public TextureOverviewView() {
            InitializeComponent();

            _selectTextBoxStyle = new Style();
            _selectTextBoxStyle.Setters.Add(new Setter(BorderBrushProperty, Brushes.CornflowerBlue));

            _selectedTextBlokGroup = [_textureDiffuseTextBox, _textureBumpTextBox, _textureSpecularTextBox];

            foreach (var textBox in _selectedTextBlokGroup) {
                textBox.GotFocus += SelectTextBoxHandler;
            }

        }

        private void SelectTextBoxHandler(object sender, RoutedEventArgs e) {
            if (sender is TextBox senderTextBox) {
                foreach (var textBox in _selectedTextBlokGroup) {
                    textBox.Style = null;
                }
                ViewModel.SetSelectFieldIndex(_selectedTextBlokGroup.FindIndex(senderTextBox));
                senderTextBox.Style = _selectTextBoxStyle;
            }
        }
    }
}
