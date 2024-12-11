using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels {
    public class TextureMaterialListViewModel : BaseViewModel {
        private PlyAggregateMtlFile? _mtlFile;
        private int _selectedMaterialIndex;

        public PlyAggregateMtlFile? MtlFile {
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