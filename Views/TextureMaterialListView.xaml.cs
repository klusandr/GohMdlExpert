using GohMdlExpert.ViewModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для TextureMaterialListView.xaml
    /// </summary>
    public partial class TextureMaterialListView : BaseView {
        public TextureMaterialListView() {
            InitializeComponent();
            DataContext = ViewModelProvider.GetRequiredViewModel<TextureMaterialListViewModel>();
        }
    }
}
