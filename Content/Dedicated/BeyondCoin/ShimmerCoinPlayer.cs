using Aequus.Content.PermaPowerups.Shimmer;
using System;

namespace Aequus.Content.Dedicated.BeyondCoin;

public class ShimmerCoinPlayer : ModPlayer {
    public override void PostUpdateEquips() {
        float effectiveness = ShimmerCoin.Effectiveness;

        // Aegis Fruit
        Player.statDefense += Math.Max((int)(4 * effectiveness), 1);

        // Gummy Worm
        Player.fishingSkill += (int)(3 * effectiveness);

        // Ambrosia
        Player.pickSpeed -= 0.05f * effectiveness;
        Player.tileSpeed += 0.05f * effectiveness;
        Player.wallSpeed += 0.05f * effectiveness;

        // Cosmic Chest
        Player.GetModPlayer<AequusPlayer>().dropRolls += CosmicChest.LuckIncrease * effectiveness;
    }

    public override void UpdateLifeRegen() {
        // Vital Crystal
        Player.lifeRegenTime += 0.2f * ShimmerCoin.Effectiveness;
    }

    public override void ModifyLuck(ref float luck) {
        // Galaxy Pearl
        luck += 0.03f * ShimmerCoin.Effectiveness;
    }
    
    // Arcane Crystal
    internal static void ArcaneCrystalEffectDelay(Player player) {
        player.manaRegenDelay -= 0.05f * ShimmerCoin.Effectiveness;
    }

    internal static void ArcaneCrystalEffect(Player player) {
        player.manaRegen += (int)(player.statManaMax2 / 50 * ShimmerCoin.Effectiveness);
    }

    // Tinkerer's Guidebook
    internal static void TinkererRerolls(ref int rerolls) {
        rerolls += (int)(TinkerersGuidebook.BonusRerolls * ShimmerCoin.Effectiveness);
    }
}
