using Aequus.Core.Graphics.ShaderData;
using Terraria.Graphics.Effects;

namespace Aequus.Content.Bosses.Crabson.Effects;

public class CrabsonScreenShader : ModSceneEffect {
    public static ScreenShaderDataWrap ScreenFilter { get; private set; } = new(typeof(CrabsonScreenShader).GetFilePath(), "Crabson", "CrabsonScreenShaderPass", EffectPriority.High);

    public override void Load() {
        if (!Main.dedServ) {
            ScreenFilter.Load()
                .UseColor(new Color(0, 10, 200))
                .UseImage(AequusTextures.EffectNoise.Value, 1)
                .UseIntensity(0.3f);
        }
    }

    public override void Unload() {
        ScreenFilter.Unload();
    }

    public override bool IsSceneEffectActive(Player player) {
        return Crabson.CrabsonBoss > -1;
    }

    public override void SpecialVisuals(Player player, bool isActive) {
        ScreenFilter.Manage(isActive);
    }
}