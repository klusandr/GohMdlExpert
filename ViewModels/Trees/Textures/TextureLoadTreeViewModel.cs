using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GohMdlExpert.Models.GatesOfHell;
using GohMdlExpert.Models.GatesOfHell.Caches;
using GohMdlExpert.Models.GatesOfHell.Resources;
using GohMdlExpert.Models.GatesOfHell.Resources.Files;
using GohMdlExpert.Models.GatesOfHell.Сaches;
using Microsoft.Extensions.DependencyInjection;
using WpfMvvm.ViewModels.Controls;

namespace GohMdlExpert.ViewModels.Trees.Textures {
    public class TextureLoadTreeViewModel : TreeViewModel {
        private TextureLoadTreeTextureViewModel? _selectedTextureItem;

        public TextureLoadTreeTextureViewModel? SelectedTextureItem {
            get => _selectedTextureItem;
            set {
                _selectedTextureItem = value;
                OnPropertyChanged();
            }
        }

        public GohResourceProvider ResourceProvider { get; }
        public GohTextureProvider TextureProvider { get; }

        public event EventHandler? TextureApply;

        public TextureLoadTreeViewModel(GohResourceProvider resourceProvider, GohTextureProvider textureProvider) {
            ResourceProvider = resourceProvider;
            TextureProvider = textureProvider;
            resourceProvider.ResourceUpdated += ResourceProviderUpdatedHandler;
            PropertyChangeHandler.AddHandler(nameof(SelectedItem), (_, _) => SelectedTextureItem = SelectedItem as TextureLoadTreeTextureViewModel);
        }

        private void ResourceProviderUpdatedHandler(object? sender, EventArgs e) {
            UpdateData();
        }

        public override void LoadData() {
            if (ResourceProvider.IsResourceLoaded) {
                var cache = GohServicesProvider.Instance.GetRequiredService<GohCacheProvider>().TexturesCache;

                

                if (cache != null) {
                    var rootDirectory = GohResourceLoading.GetResourceStructure(cache.Values.First(), ResourceProvider);

                    if (GohResourceLoading.TryGetNextCompletedDirectory(rootDirectory, out var nextDirectory, out _)) {
                        rootDirectory = nextDirectory;
                    }

                    foreach (var directory in rootDirectory.GetDirectories().ToArray()) {
                        var mtlFiles = directory.FindResourceElements<MtlFile>(deepSearch: true);

                        TextureProvider.SetTexturesMaterialsFullPath(mtlFiles.Select(mF => mF.Data));

                        directory.Items.Clear();
                        directory.Items.AddRange(mtlFiles.OrderBy(f => f.Name));

                        AddItem(new TextureLoadTreeDirectoryViewModel(directory, this));
                    }
                }
            }
        }

        public void OnTextureApply() {
            TextureApply?.Invoke(this, EventArgs.Empty);
        }
    }
}
