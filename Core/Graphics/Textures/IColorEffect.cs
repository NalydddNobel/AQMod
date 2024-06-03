namespace Aequus.Core.Graphics.Textures;

public interface IColorEffect {
    Color GetColor(in ColorEffectContext context);
}

public record struct ColorEffectContext(Color[] Colors, int TextureWidth, int TextureHeight) {
    public int index;

    public readonly Color this[int index] => Colors[index];

    public readonly Color Color => Colors[index];
}