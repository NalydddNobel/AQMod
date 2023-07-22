using Aequus.Common.Effects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.Crabson.Effects;

public class CrabsonSceneEffect : ModSceneEffect {
    public static ScreenFliterEffect ScreenFilter { get; private set; }

    public override void Load() {
        if (!Main.dedServ) {
            ScreenFilter = new(this.NamespacePath() + "/CrabsonScreenShader", "Crabson", "CrabsonScreenShaderPass", EffectPriority.High);
            ScreenFilter.Load()
                .UseColor(new Color(0, 10, 200))
                .UseImage(AequusTextures.EffectNoise.Value, 1)
                .UseIntensity(0.3f);
        }
    }

    public override void Unload() {
        ScreenFilter = null;
    }

    public override bool IsSceneEffectActive(Player player) {
        return AequusSystem.CrabsonNPC != -1;
    }

    public override void SpecialVisuals(Player player, bool isActive) {
        ScreenFilter.Manage(isActive);
    }
}