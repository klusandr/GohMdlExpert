using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GohMdlExpert.ViewModels.Trees.OverviewModels;
using WpfMvvm.Views.Attributes;
using WpfMvvm.Views.Controls;

namespace GohMdlExpert.Views.Trees {
    /// <summary>
    /// Логика взаимодействия для ModelsOverviewTreeItemView.xaml
    /// </summary>
    [BindingViewModel<ModelsOverviewTreeItemViewModel>]
    public partial class ModelsOverviewTreeItemView : TreeItemView {
        private static TextBox? s_textBox;

        public new ModelsOverviewTreeItemViewModel ViewModel => (ModelsOverviewTreeItemViewModel)base.ViewModel;

        public ModelsOverviewTreeItemView() {
            InitializeComponent();
            DataContextChanged += DataContextChangedHandler;
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e) {
            ViewModel.PropertyChangeHandler.AddHandler(nameof(ViewModel.IsEdit), IsEditChangeHandler);
        }

        private TextBox GetTextBox() {
            if (s_textBox == null) {
                s_textBox = new TextBox();
                s_textBox.LostFocus += TexBoxLostFocusHandler;
                s_textBox.KeyDown += KeyDownHandler;
            }
            s_textBox.Text = ViewModel.Text;
            return s_textBox;
        }


        private void IsEditChangeHandler(object? sender, PropertyChangedEventArgs e) {
            if (ViewModel.IsEdit) {
                _stackPanel.Children.RemoveAt(1);
                _stackPanel.Children.Insert(1, GetTextBox());
            } else {
                _stackPanel.Children.RemoveAt(1);
                _stackPanel.Children.Insert(1, _textBlock);
            }
        }

        private void EndEdit() {
            ViewModel.Text = s_textBox!.Text;
            ViewModel.IsEdit = false;
        }

        private void CancelEdit() {
            ViewModel.IsEdit = false;
        }

        private void TexBoxLostFocusHandler(object sender, RoutedEventArgs e) {
            EndEdit();
        }

        private void KeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e) {
            if (ViewModel.IsEdit) {
                switch (e.Key) {
                    case Key.Enter: EndEdit(); break;
                    case Key.Escape: CancelEdit(); break;
                }
            }
        }
    }
}
