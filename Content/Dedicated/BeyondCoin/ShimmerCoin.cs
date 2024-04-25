using Aequus.Common.Items;
using Aequus.Common.Items.Dedications;

namespace Aequus.Content.Dedicated.BeyondCoin;

public class ShimmerCoin : ModItem {
    public static int LifeRegenIncrease { get; set; } = 2; // +1 HP/s
    public static int DefenseIncrease { get; set; } = 4;
    public static int AggroIncrease { get; set; } = -80;
    public static float MoveSpeedIncrease { get; set; } = 0.05f; // 5%
    public static float ManaCostMultiplier { get; set; } = 0.9f; // -10%
    public static float PickSpeedIncrease { get; set; } = -0.15f; // 15%
    public static float MeleeSpeedIncrease { get; set; } = 0.1f; // 10%
    public static float SummonKBIncrease { get; set; } = 0.1f; // 10%
    public static float DamageIncrease { get; set; } = 0.1f; // 10%
    public static float CritChanceIncrease { get; set; } = 2f; // 2%
    public static float WingTimeMultiplierIncrease { get; set; } = 0.1f; // 10%

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
        AequusRecipes.ShimmerTransformLocks[Type] = Condition.DownedMoonLord;
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
        player.lifeRegen += LifeRegenIncrease;
        player.statDefense += DefenseIncrease;
        player.aggro += AggroIncrease;
        player.moveSpeed += MoveSpeedIncrease;
        player.manaCost *= ManaCostMultiplier;
        player.pickSpeed += PickSpeedIncrease;

        player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedIncrease;
        player.GetKnockback(DamageClass.Summon) += SummonKBIncrease;
        player.GetCritChance(DamageClass.Generic) += CritChanceIncrease;
        player.GetDamage(DamageClass.Generic) += DamageIncrease;

        if (!player.TryGetModPlayer(out AequusPlayer aequus)) { return; }

        aequus.wingTime += WingTimeMultiplierIncrease;
    }
}
