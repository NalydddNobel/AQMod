using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Items.Accessories.CrownOfBlood.Buffs;
using Aequus.Items.Accessories.CrownOfBlood.Hearts;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer : ModPlayer {
    public bool accCrownOfBloodAntiCheat;
    public Item accCrownOfBlood;
    public Item accCrownOfBloodItemClone;

    private int crownOfBloodHearts;
    /// <summary>
    /// The amount of hearts consumed by the <see cref="CrownOfBloodItem"/>.
    /// </summary>
    public int CrownOfBloodHearts { get => crownOfBloodHearts; set => crownOfBloodHearts = Math.Clamp(value, 0, Player.TotalHearts() - 1); }
    public int crownOfBloodRegenTime;

    private void ResetEffects_CrownOfBlood() {
        accCrownOfBloodAntiCheat = accCrownOfBlood == null;
        accCrownOfBlood = null;
        accCrownOfBloodItemClone = null;
        crownOfBloodBees = 0;
        crownOfBloodDeerclops = 0;
        crownOfBloodFriendlySlimes = 0;
    }

    private void ClearCrownOfBlood() {
        ResetEffects_CrownOfBlood();
        crownOfBloodHearts = 0;
        crownOfBloodRegenTime = 0;
        crownOfBloodCD = 0;
    }

    private void UpdateCrownOfBlood() {
        if (Main.myPlayer == Player.whoAmI) {
            CrownOfBloodItem.UpdateEquipEffect(accCrownOfBlood != null, accCrownOfBloodItemClone);
        }

        if (crownOfBloodHearts <= 0) {
            crownOfBloodRegenTime = 0;
            return;
        }

        if (Main.myPlayer == Player.whoAmI) {
            CrownOfBloodHeartsOverlay.AnimationTimer += 2f + crownOfBloodHearts / 5f;
        }

        if (timeSinceLastHit > 300) {
            crownOfBloodRegenTime++;
            if (crownOfBloodRegenTime > 50) {
                crownOfBloodRegenTime = 0;

                crownOfBloodHearts--;
                if (crownOfBloodHearts <= 0) {
                    return;
                }
            }
        }
        else {
            crownOfBloodRegenTime = 0;
        }

        int hearts = Player.TotalHearts();
        int heartsLeft = Math.Max(hearts - crownOfBloodHearts, 1);
        Player.statLife = Math.Min(Player.statLife, Player.statLifeMax2 / hearts * heartsLeft);
    }

    private void PostUpdateEquips_CrownOfBlood() {
        if (crownOfBloodCD > 0) {
            crownOfBloodCD--;
            Player.AddBuff(ModContent.BuffType<CrownOfBloodCooldown>(), crownOfBloodCD);
        }

        PostUpdateEquips_WormScarfEmpowerment();
        PostUpdateEquips_BoneHelmEmpowerment();
        PostUpdateEquips_RoyalGels();
    }

    private void InflictCrownOfBloodDownside(Player.HurtInfo hit) {
        if (accCrownOfBlood != null) {
            CrownOfBloodHearts += hit.Damage / Player.HealthPerHeart();
        }
    }
}