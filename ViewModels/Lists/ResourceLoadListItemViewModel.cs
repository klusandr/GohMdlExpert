using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Properties;
using WpfMvvm.ViewModels;

namespace GohMdlExpert.ViewModels.Lists {
    public class ResourceLoadListItemViewModel : BaseViewModel {
        private ImageSource? _icon;
        private string? _text;
        private string? _toolTip;

        public GohResourceElement ResourceElement { get; }

        public string? Text {
            get => _text;
            set {
                _text = value;
                OnPropertyChanged();
            }
        }

        public ImageSource? Icon {
            get => _icon;
            set {
                _icon = value;
                OnPropertyChanged();
            }
        }

        public string? ToolTip {
            get => _toolTip;
            set {
                _toolTip = value;
                OnPropertyChanged();
            }
        }

        public ICommand ApproveCommand => CommandManager.GetCommand(Approve);

        public event EventHandler? ApproveEvent;

        public ResourceLoadListItemViewModel(GohResourceElement resourceElement) {
            ResourceElement = resourceElement;
            Text = resourceElement.Name;

            string iconRexource = resourceElement switch {
                GohResourceDirectory => nameof(Resources.DirectoryIcon),
                MdlFile => nameof(Resources.MdlIcon),
                PlyFile => nameof(Resources.PlyIcon),
                MtlFile => nameof(Resources.TextureIcon),
                MaterialFile => nameof(Resources.TextureIcon),
                _ => nameof(Resources.PlyIcon),
            };

            Icon = IconResources.Instance.GetIcon(iconRexource);
        }

        private void Approve() {
            ApproveEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
