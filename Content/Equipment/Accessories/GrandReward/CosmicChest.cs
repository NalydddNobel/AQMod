namespace Aequus.Content.Equipment.Accessories.GrandReward;

[LegacyName("GalaxyCommission", "Moro")]
public class CosmicChest : ModItem {
    public static float LuckIncrease { get; set; } = 0.05f;

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<GrandReward>()] = Type;
    }

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

    public override bool? UseItem(Player player) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        if (aequusPlayer.usedCosmicChest) {
            return false;
        }
        aequusPlayer.usedCosmicChest = true;
        return true;
    }
}