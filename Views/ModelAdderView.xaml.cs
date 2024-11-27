using GohMdlExpert.ViewModels.LoadModels;
using WpfMvvm.Views;

namespace GohMdlExpert.Views {
    /// <summary>
    /// Логика взаимодействия для ModelAdderView.xaml
    /// </summary>
    public partial class ModelAdderView : BaseView {
        public ModelAdderView() {
            InitializeComponent();

            ViewModel = ViewModelProvider.GetRequiredViewModel<ModelAdderViewModel>();
        }
    }
}
