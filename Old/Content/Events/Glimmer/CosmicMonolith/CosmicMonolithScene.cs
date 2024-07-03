using Aequu2.Old.Content.Events.Glimmer.Sky;
using Terraria.Graphics.Effects;

namespace Aequu2.Old.Content.Events.Glimmer.CosmicMonolith;

public class CosmicMonolithScene : ModSceneEffect {
    public const string Key = "Aequu2:CosmicMonolith";

    public static bool MonolithNearby { get; set; }

    public override bool IsSceneEffectActive(Player player) {
        return MonolithNearby || player.GetModPlayer<Aequu2Player>().cosmicMonolithShader;
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