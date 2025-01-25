using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files.Aggregates;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class TextureMaterialListViewModel : BaseViewModel {
        private AggregateMtlFile? _mtlFile;
        private int _selectedMaterialIndex;

        public AggregateMtlFile? MtlFile {
            get => _mtlFile;
            set {
                _mtlFile = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Items));
            }
        }

        public MtlTextureCollection? Items => MtlFile?.Data;

        public int SelectedMaterialIndex {
            get => _selectedMaterialIndex;
            set {
                _selectedMaterialIndex = value;
                OnPropertyChanged();
            }
        }

        public TextureMaterialListViewModel() { }
    }
}