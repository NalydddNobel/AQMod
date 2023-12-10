using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

public class EtOmniaVanitasPlayer : ModPlayer {
    public const string TimerId = "EtOmniaVanitasChargeTime";

    public int chargeProgress;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Player.SetTimer(TimerId, 30);
        EtOmniaVanitasBar.HitAnimation = 1f;
    }

    public override void PreUpdate() {
        if (chargeProgress < EtOmniaVanitas.MaxChargeProgress && Player.TimerActive(TimerId)) {
            chargeProgress++;
            if (chargeProgress >= EtOmniaVanitas.MaxChargeProgress) {
                Player.SetTimer(TimerId + "Complete", 24);
            }
        }
        if (Main.mouseRight && chargeProgress > 0) {
            chargeProgress--;
        }
    }
}