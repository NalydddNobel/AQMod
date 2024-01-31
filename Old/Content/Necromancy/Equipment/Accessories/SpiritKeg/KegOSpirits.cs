using Terraria.Localization;

namespace Aequus.Old.Content.Necromancy.Equipment.Accessories.SpiritKeg;

public class KegOSpirits : ModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Minutes(SaivoryKnife.GhostLifespan), BottleOSpirits.GhostSlotIncrease);

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequus = player.GetModPlayer<AequusPlayer>();
        aequus.ghostLifespan += 3600;
        aequus.ghostSlotsMax++;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<BottleOSpirits>()
            .AddIngredient<SaivoryKnife>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PapyrusScarab);
    }
}
