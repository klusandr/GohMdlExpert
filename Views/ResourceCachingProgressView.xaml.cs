using System.Windows.Controls;
using GohMdlExpert.ViewModels;
using WpfMvvm.Views.Attributes;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для ResourceCachedProgressView.xaml
    /// </summary>
    [BindingViewModel<ResourceCachingProgressViewModel>]
    public partial class ResourceCachingProgressView : UserControl {
        public ResourceCachingProgressView() {
            InitializeComponent();
        }
    }
}
