using System.Windows.Controls;
using GohMdlExpert.ViewModels.SettingsPages;
using WpfMvvm.Views;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views.SettingsPages {
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    [BindingViewModel<SettingsViewModel>]
    [BindingViewModelViaDI]
    public partial class SettingsView : BaseView {

        public new SettingsViewModel ViewModel => (SettingsViewModel)base.ViewModel;

        public SettingsView() {
            InitializeComponent();

            foreach (var item in _tabControl.Items.Cast<TabItem>()) {
                if (item.Content is BaseView view) {
                    var viewModelAttribute = BindingViewModelAttribute.GetAttribute(view.GetType());

                    if (viewModelAttribute != null) {
                        view.ViewModel = ViewModel.GetPage(viewModelAttribute.ViewModelType)
                            ?? throw new WpfMvvm.Exceptions.MvvmComponentInitializeException($"The view model for settings page is not define in {nameof(SettingsViewModel)}", view.GetType());
                    }
                }
            }
        }

        public void OpenPage(string pageName) {
            foreach (var item in _tabControl.Items.Cast<TabItem>()) {
                if (item.Content is SettingsPageView pageView) {
                    if (pageView.PageName == pageName) {
                        _tabControl.SelectedItem = item;
                        return;
                    }
                }
            }
        }
    }
}
