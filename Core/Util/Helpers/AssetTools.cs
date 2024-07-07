using ReLogic.Content;

namespace AequusRemake.Core.Util.Helpers;

public static class AssetTools {
    public static T ForceLoad<T>(ref Asset<T> Asset) where T : class {
        if (Asset.IsLoaded) {
            return Asset.Value;
        }
        if (Asset.State == AssetState.NotLoaded) {
            Asset = ModContent.Request<T>(Asset.Name);
        }
        Asset.Wait?.Invoke();
        return Asset.Value;
    }
}
