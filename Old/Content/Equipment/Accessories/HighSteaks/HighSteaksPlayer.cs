using Aequus.Old.Core.Assets;
using Terraria.Audio;

namespace Aequus.Old.Content.Equipment.Accessories.HighSteaks;

public class HighSteaksPlayer : ModPlayer {
    public float highSteaksDamage;
    public int highSteaksCost;
    public bool highSteaksHidden;

    public override void ResetEffects() {
        highSteaksHidden = false;
        highSteaksCost = 0;
        highSteaksDamage = 0f;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        if (highSteaksCost == 0 || target.immortal || CanAffordWithInventoryMoney(highSteaksCost)) {
            modifiers.CritDamage += highSteaksDamage;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (hit.Crit) {
            UseHighSteaks(target);
        }
    }

    public void UseHighSteaks(NPC target) {
        if (highSteaksDamage <= 0f) {
            return;
        }

        if (Main.rand.NextBool(8)) {
            SoundEngine.PlaySound(OldAequusSounds.HighSteaks, target.Center);
        }

        if (highSteaksCost != 0) {
            if (!CanAffordWithInventoryMoney(highSteaksCost)) {
                return;
            }

            SoundEngine.PlaySound(AequusSounds.HighSteaksCriticalStrike with { PitchVariance = 0.2f }, target.Center);
            if (!target.immortal) {
                // This should only consume coins in the inventory
                // since we checked earlier if the inventory has the desired amount of coins.
                Player.BuyItem(highSteaksCost);
            }
            HighSteaksMoneyConsumeEffect.CoinAnimations.Add(Main.rand.Next((int)(MathHelper.TwoPi * 100f)) * 100);
        }
    }

    private bool CanAffordWithInventoryMoney(long wantedValue) {
        long count = Utils.CoinsCount(out _, Player.inventory, 58, 57, 56, 55, 54);
        return count >= wantedValue;
    }
}
