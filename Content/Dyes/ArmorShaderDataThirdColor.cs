using Terraria.Graphics.Shaders;

namespace Aequus.Content.Dyes;

/// <summary>
/// Uses "uLightSource" as a 3rd color value.
/// </summary>
public sealed class ArmorShaderDataThirdColor : ArmorShaderData {
    public Vector3 _thirdColor;

    public ArmorShaderDataThirdColor(Ref<Effect> shader, System.String passName, Vector3 thirdColor) : base(shader, passName) {
        _thirdColor = thirdColor;
    }

    public ArmorShaderDataThirdColor(Ref<Effect> shader, System.String passName, Color thirdColor) : base(shader, passName) {
        _thirdColor = thirdColor.ToVector3();
    }

    public override void Apply() {
        Shader.Parameters["uLightSource"].SetValue(_thirdColor);
        base.Apply();
    }
}