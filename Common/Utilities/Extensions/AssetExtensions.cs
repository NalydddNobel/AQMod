using ReLogic.Content;

namespace Aequus.Common.Utilities.Extensions;

public static class AssetExtensions {
    public static void Request<T>(this Asset<T> asset) where T : class {
        if (asset.IsLoaded) {
            return;
        }

        string path = asset.Name;

        // This also indirectly updates our asset instance.
        ModContent.RequestIfExists<Texture2D>(asset.Name, out _);
    }
}
