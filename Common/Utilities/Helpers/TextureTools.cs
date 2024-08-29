namespace Aequus.Common.Utilities.Helpers;

public static class TextureTools {
    public static Color[] AllocColorData(Texture2D texture) {
        Color[] colors = new Color[texture.Width * texture.Height];

        texture.GetData(colors);

        return colors;
    }
}
