using System;

namespace Aequus.Content.Dedicated.EtOmniaVanitas;

public class EtOmniaVanitasPlayer : ModPlayer {
    public const string TimerId = "EtOmniaVanitasChargeTime";
    public const string TimerId_Complete = "EtOmniaVanitasChargeFinished";

    public int chargeProgress;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Player.SetTimer(TimerId, 30);
        //EtOmniaVanitasBar.HitAnimation = 1f;
        EtOmniaVanitas.MaxChargeProgress = 2000;
    }

    public override void PreUpdate() {
        if (chargeProgress < EtOmniaVanitas.MaxChargeProgress && Player.TryGetTimer(TimerId, out var timer) && timer.Active) {
            chargeProgress += (int)Math.Max((timer.MaxTime - timer.TimePassed) / 2f, 1);
            if (chargeProgress >= EtOmniaVanitas.MaxChargeProgress) {
                chargeProgress = EtOmniaVanitas.MaxChargeProgress;
                Player.SetTimer(TimerId_Complete, 24);
            }
        }
        if (Main.mouseRight && chargeProgress > 0) {
            chargeProgress /= 2;
            chargeProgress--;
            if (chargeProgress <= 0) {
                chargeProgress = 0;
            }
        }
    }
}