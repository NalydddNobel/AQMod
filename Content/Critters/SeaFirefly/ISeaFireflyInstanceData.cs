using Aequus.Core.Graphics.Textures;

namespace Aequus.Content.Critters.SeaFirefly;

public readonly record struct GlowColorContext(Vector2 Position, int RandomSeed);

public interface ISeaFireflyInstanceData {
    byte Type { get; set; }
    string Name { get; set; }
    IColorEffect ColorEffect { get; }
    IColorEffect TileEffect => ColorEffect;
    Vector3 GetLightColor(Vector2 position);
    Color GetBugColor();
    Color GetGlowColor(GlowColorContext context);
}

public record struct DyeVariant(string Name, Color Color) : ISeaFireflyInstanceData {
    byte ISeaFireflyInstanceData.Type { get; set; }
    string ISeaFireflyInstanceData.Name { get; set; } = Name;

    readonly IColorEffect ISeaFireflyInstanceData.ColorEffect => new EffectHSLShift(Main.rgbToHsl(Color));

    readonly Vector3 ISeaFireflyInstanceData.GetLightColor(Vector2 Location) {
        return Color.ToVector3();
    }

    readonly Color ISeaFireflyInstanceData.GetBugColor() {
        return Color;
    }

    readonly Color ISeaFireflyInstanceData.GetGlowColor(GlowColorContext context) {
        return Color;
    }
}

public record struct DefaultVariant(string Name) : ISeaFireflyInstanceData {
    byte ISeaFireflyInstanceData.Type { get; set; }
    string ISeaFireflyInstanceData.Name { get; set; } = Name;

    readonly IColorEffect ISeaFireflyInstanceData.ColorEffect => null;

    public Vector3 GetLightColor(Vector2 Location) {
        return new Vector3(0.1f, 0.2f, 0.3f);
    }

    public Color GetBugColor() {
        return Color.Transparent;
    }

    public Color GetGlowColor(GlowColorContext context) {
        float wave = Helper.Oscillate(context.RandomSeed + context.Position.X * 0.01f + Main.GlobalTimeWrappedHourly * 4f, 1f);

        return Color.Lerp(new Color(30, 90, 255, 50), new Color(40, 255, 255, 50), wave);
    }
}

public record struct RainbowVariant(string Name) : ISeaFireflyInstanceData {
    byte ISeaFireflyInstanceData.Type { get; set; }
    string ISeaFireflyInstanceData.Name { get; set; } = Name;

    readonly IColorEffect ISeaFireflyInstanceData.ColorEffect => new EffectHorizontalRainbow();
    readonly IColorEffect ISeaFireflyInstanceData.TileEffect => new EffectVelocityGrayscale();

    public Vector3 GetLightColor(Vector2 Location) {
        return GetGlowColor(new GlowColorContext(Location, 0)).ToVector3();
    }

    public Color GetBugColor() {
        return Main.DiscoColor;
    }

    public Color GetGlowColor(GlowColorContext context) {
        float wave = Helper.Oscillate(context.Position.X * 0.01f + Main.GlobalTimeWrappedHourly, 1f);

        return ExtendColor.HueSet(Color.Red, wave) with { A = 0 };
    }
}
