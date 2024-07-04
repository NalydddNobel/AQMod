using ReLogic.Content;

namespace AequusRemake.Core.Graphics.Textures;

public interface IColorEffect {
    void Prepare(in ColorEffectContext context) { }
    Color GetColor(in ColorEffectContext context);
}

public record struct ColorEffectContext(Texture2D Texture, Color[] Colors) {
    public int index;
    public readonly int TextureWidth => Texture.Width;
    public readonly int TextureHeight => Texture.Height;

    public readonly Color this[int index] => Colors[index];

    public readonly Color Color => Colors[index];

    public static ColorEffectContext FromTexture(Asset<Texture2D> Texture) {
        return FromTexture(ExtendTexture.Wait(Texture));
    }

    public static ColorEffectContext FromTexture(Texture2D Texture) {
        Color[] colors = new Color[Texture.Width * Texture.Height];

        Texture.GetData(colors);

        return new ColorEffectContext(Texture, colors);
    }
}