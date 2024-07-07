namespace AequusRemake.Core.Util.Extensions;

public static class TextureExtensions {
    public static Color[] AllocData(Texture2D texture) {
        Color[] colors = new Color[texture.Width * texture.Height];

        texture.GetData(colors);

        return colors;
    }
}
