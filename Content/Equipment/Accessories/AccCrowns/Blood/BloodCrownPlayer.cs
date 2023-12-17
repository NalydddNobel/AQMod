using Aequus.Content.Equipment.Accessories.AccCrowns.Blood;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus;

public class BloodCrownPlayer : ModPlayer {
    public bool accBloodCrown;
    public bool accBloodCrownOld;

    public override void ResetEffects() {
        accBloodCrownOld = accBloodCrown;
        accBloodCrown = false;
    }

    private void UpdateBloodCrownInner(Item equip) {
        var player = Player;
        player.ApplyEquipFunctional(equip, player.hideVisibleAccessory[BloodCrown.SlotId]);
        if (equip.wingSlot != -1) {
            player.wingTimeMax *= 2;
        }
    }

    public override void UpdateEquips() {
        UpdateBloodHearts();
        if (!accBloodCrown) {
            BloodCrown.TooltipUUID = 0;
            return;
        }

        var player = Player;
        var equip = player.armor[BloodCrown.SlotId];
        if (equip == null || equip.IsAir) {
            BloodCrown.TooltipUUID = 0;
            return;
        }

        if (Main.myPlayer == player.whoAmI && (BloodCrown.statTickUpdate == 0 || BloodCrown.TooltipUUID == 0)) {
            // Update UUID
            if (equip.TryGetGlobalItem<BloodCrownGlobalItem>(out var globalItem) && (globalItem._localUUID != BloodCrown.TooltipUUID || BloodCrown.TooltipUUID == 0)) {
                int uuid = Main.rand.Next();
                BloodCrown.TooltipUUID = uuid;
                globalItem._localUUID = uuid;
            }

            // Record previous stats for comparison
            BloodCrown.StatComparer.Record(player);

            int prefix = equip.prefix;
            equip.prefix = 0;
            try {
                UpdateBloodCrownInner(equip);
            }
            catch {
            }
            equip.prefix = prefix;

            // Measure before-and-after stats for tooltip
            BloodCrown.StatComparer.Measure(player);

            BloodCrown.statTickUpdate = BloodCrown.StatTickUpdateRate;
        }
        else {
            if (Main.myPlayer == player.whoAmI && BloodCrown.statTickUpdate > 0) {
                BloodCrown.statTickUpdate--;
            }
            UpdateBloodCrownInner(equip);
        }
    }

    #region Hearts
    private int bloodHearts;
    public int BloodHearts { get => bloodHearts; set => bloodHearts = Math.Clamp(value, 0, Player.ConsumedLifeCrystals + 4); }

    public int bloodHeartRegen;

    private void UpdateBloodHearts() {
        if (bloodHearts <= 0) {
            bloodHeartRegen = 0;
            return;
        }

        if (Main.myPlayer == Player.whoAmI) {
            BloodHeartsOverlay.AnimationTimer += 2f + bloodHearts / 5f;
        }

        if (Player.GetModPlayer<AequusPlayer>().timeSinceLastHit > 300) {
            bloodHeartRegen++;
            if (bloodHeartRegen > 50) {
                bloodHeartRegen = 0;

                bloodHearts--;
                if (bloodHearts <= 0) {
                    return;
                }
            }
        }
        else {
            bloodHeartRegen = 0;
        }

        int hearts = Player.HeartCount();
        int heartsLeft = Math.Max(hearts - bloodHearts, 1);
        Player.statLife = Math.Min(Player.statLife, Player.statLifeMax2 / hearts * heartsLeft);
    }

    public override void OnHurt(Player.HurtInfo info) {
        if (accBloodCrown) {
            BloodHearts += (int)(info.Damage / Player.HealthPerHeart());
        }
    }
    #endregion
}