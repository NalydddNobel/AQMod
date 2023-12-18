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
        bloodCrownEndurance = 0f;
    }

    private void UpdateBloodCrownInner(Item equip) {
        var player = Player;

        float damageReductionOld = player.endurance;

        player.ApplyEquipFunctional(equip, player.hideVisibleAccessory[BloodCrown.SlotId]);

        // Move any extra "Endurance" (Damage Reduction) to a custom variable
        // Prevents the vanilla "endurance" stat from having any chance to go over 1.
        float damageReductionDifference = player.endurance - damageReductionOld;
        if (damageReductionDifference > 0f) {
            player.endurance = damageReductionOld;
            bloodCrownEndurance += damageReductionDifference;
        }

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

    #region Unique interactions
    public float bloodCrownEndurance;

    public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
        modifiers.FinalDamage *= Helper.CappedExponential(bloodCrownEndurance);
    }
    #endregion
}