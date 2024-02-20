namespace Aequus.Content.PermaPowerups.NoHit;

[LegacyName("VictorsReward")]
public class NoHitReward : ModItem {
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
        if (aequusPlayer.usedMaxHPRespawnReward) {
            return false;
        }
        aequusPlayer.usedMaxHPRespawnReward = true;
        return true;
    }
}