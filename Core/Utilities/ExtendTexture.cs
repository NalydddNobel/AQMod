using ReLogic.Content;

namespace Aequu2.Core.Utilities;

public static class ExtendTexture {
    public static Texture2D Wait(Asset<Texture2D> Asset) {
        Asset.Wait?.Invoke();
        return Asset.Value;
    }

    public static Color[] GetDataFull(Texture2D texture) {
        Color[] colors = new Color[texture.Width * texture.Height];

        texture.GetData(colors);

        return colors;
    }
}
