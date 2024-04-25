using Aequus.Common.Items.Dedications;

namespace Aequus.Content.Dedicated.BeyondCoin;

public class ShimmerCoin : ModItem {
    public static float CoinRangeIncrease { get; set; } = 80f;

    public override void SetDefaults() {
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void SetStaticDefaults() {
        DedicationRegistry.RegisterSubItem(ModContent.GetInstance<BeyondPlatinumCoin>(), this);
    }

    public override bool? UseItem(Player player) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        if (aequusPlayer.usedShimmerCoin) {
            return false;
        }

        aequusPlayer.usedShimmerCoin = true;
        return true;
    }

    public static void UpdatePermanentEffects(Player player) {
        player.lifeRegen += 2; // +1 HP/s
        player.statDefense += 2;
        player.aggro -= 80;
        player.moveSpeed += 0.05f;
        player.manaCost *= 0.9f;

        player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
        player.GetKnockback(DamageClass.Generic) += 0.2f;
        player.GetCritChance(DamageClass.Generic) += 2f;
        player.GetDamage(DamageClass.Generic) += 0.05f;

        if (!player.TryGetModPlayer(out AequusPlayer aequus)) { return; }

        aequus.wingTime += 0.05f;
    }
}
