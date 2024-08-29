using Aequus.Common.Utilities.Helpers;
using ReLogic.Content;

namespace Aequus.Common.Drawing.Generative;

public interface ITextureGenerator {
    void Prepare(in TextureGenContext context) { }
    Color[] GenerateData(ref TextureGenContext context);
}

public struct TextureGenContext {
    public Texture2D Texture;
    public Color[] Colors;
    public int TextureWidth { get; set; }
    public int TextureHeight { get; set; }

    public TextureGenContext(Texture2D Texture, Color[] Colors) {
        this.Texture = Texture;
        this.Colors = Colors;
        TextureWidth = Texture.Width;
        TextureHeight = Texture.Height;
    }

    public readonly Color this[int index] => Colors[index];

    public static TextureGenContext FromTexture(Asset<Texture2D> Texture) {
        return FromTexture(AssetTools.ForceLoad(ref Texture));
    }

    public static TextureGenContext FromTexture(Texture2D Texture) {
        Color[] colors = new Color[Texture.Width * Texture.Height];

        Texture.GetData(colors);

        return new TextureGenContext(Texture, colors);
    }
}