using Aequus.Content.Events.GlimmerEvent.Sky;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Tiles.Monoliths.CosmicMonolith;

public class CosmicMonolithScene : ModSceneEffect {
    public const string Key = "Aequus:CosmicMonolith";

    public static bool MonolithNearby;

    public override bool IsSceneEffectActive(Player player) {
        return MonolithNearby || player.Aequus().cosmicMonolithShader;
    }

    public override void Load() {
        if (!Main.dedServ) {
            SkyManager.Instance[Key] = new GlimmerSky() { checkDistance = false, };
        }
    }

    public override void SpecialVisuals(Player player, bool isActive) {
        if (isActive) {
            if (!SkyManager.Instance[Key].IsActive()) {
                SkyManager.Instance.Activate(Key);
            }
        }
        else {
            if (SkyManager.Instance[Key].IsActive()) {
                SkyManager.Instance.Deactivate(Key);
            }
        }
    }
}