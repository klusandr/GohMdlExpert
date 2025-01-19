using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                s_textBox.LostFocus += TexBoxLostFocus;
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

        private void TexBoxLostFocus(object sender, RoutedEventArgs e) {
            ViewModel.Text = s_textBox!.Text;
            ViewModel.IsEdit = false;
        }
    }
}
