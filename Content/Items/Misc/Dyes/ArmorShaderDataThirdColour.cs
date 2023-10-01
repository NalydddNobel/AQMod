using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Misc.Dyes;

/// <summary>
/// Uses "uLightSource" as a 3rd color value.
/// </summary>
public class ArmorShaderDataThirdColour : ArmorShaderData {
    public Vector3 _thirdColor;

    public ArmorShaderDataThirdColour(Ref<Effect> shader, string passName, Vector3 thirdColor) : base(shader, passName) {
        _thirdColor = thirdColor;
    }

    public ArmorShaderDataThirdColour(Ref<Effect> shader, string passName, Color thirdColor) : base(shader, passName) {
        _thirdColor = thirdColor.ToVector3();
    }

    public override void Apply() {
        Shader.Parameters["uLightSource"].SetValue(_thirdColor);
        base.Apply();
    }
}