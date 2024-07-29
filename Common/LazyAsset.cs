using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common;

public sealed class LazyAsset<T> where T : class {
    public readonly string Path;

    private IAssetProvider _assetProvider;

    public string FullPath => $"{nameof(Aequus)}/{Path}";

    private string? _name;
    public string Name => _name ??= FullPath.Split('/').Last().Trim();

    public Asset<T> Asset => _assetProvider.Asset;

    public T Value => Asset.Value;

    public LazyAsset(string path) {
        Path = path;
        _assetProvider = new UnloadedAssetProvider(this);
#if DEBUG
        PreloadSystem._queue.Add(this);
#endif
    }

    public void Preload() {
        _assetProvider.PreLoad();
    }

    public static implicit operator Asset<T>(LazyAsset<T> value) {
        return value.Asset;
    }

    public static implicit operator T(LazyAsset<T> value) {
        return value.Value;
    }

    /// <summary>Provides an asset. Comes in a loaded and unloaded variety to reduce on null checks.</summary>
    private interface IAssetProvider {
        Asset<T> Asset { get; }
        void PreLoad();
    }

    /// <summary>Represents an Unloaded Asset.</summary>
    /// <param name="asset"></param>
    public readonly struct UnloadedAssetProvider(LazyAsset<T> asset) : IAssetProvider {
        Asset<T> LazyAsset<T>.IAssetProvider.Asset {
            get {
                LoadAsset(AssetRequestMode.ImmediateLoad);
                return asset.Asset;
            }
        }

        void LazyAsset<T>.IAssetProvider.PreLoad() {
            LoadAsset(AssetRequestMode.AsyncLoad);
        }

        void LoadAsset(AssetRequestMode mode) {
            Asset<T> resultAsset = ModContent.Request<T>(asset.FullPath, mode);
            asset._assetProvider = new AssetProvider(resultAsset);
        }
    }

    /// <summary>Represents a loaded asset.</summary>
    /// <param name="asset"></param>
    public readonly struct AssetProvider(Asset<T> asset) : IAssetProvider {
        readonly Asset<T> LazyAsset<T>.IAssetProvider.Asset => asset;

        // Asset is already loaded, no need to do anything in preload function.
        void LazyAsset<T>.IAssetProvider.PreLoad() { }
    }

#if DEBUG
    private class PreloadSystem : ModSystem {
        public static readonly List<LazyAsset<T>> _queue = [];

        // Preload lazy assets to ensure they connect to a texture.
        public override void Load() {
            foreach (var item in _queue) {
                item.Preload();
            }
        }

        public override void Unload() {
            _queue.Clear();
        }
    }
#endif
}
