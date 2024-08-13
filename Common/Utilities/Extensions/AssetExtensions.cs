using ReLogic.Content;

namespace Aequus.Common.Utilities.Extensions;

public static class AssetExtensions {
    public static void Request<T>(this Asset<T> asset, Mod? mod = null) where T : class {
        if (asset.IsLoaded) {
            return;
        }

        mod ??= Aequus.Instance;
        string path = asset.Name;

        // This also indirectly updates our asset instance.
        ModContent.RequestIfExists<Texture2D>($"{mod.Name}/{asset.Name}", out _);
    }
}
