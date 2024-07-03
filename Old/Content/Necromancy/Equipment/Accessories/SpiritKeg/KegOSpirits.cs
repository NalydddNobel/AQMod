using Terraria.Localization;

namespace Aequu2.Old.Content.Necromancy.Equipment.Accessories.SpiritKeg;

public class KegOSpirits : ModItem {
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(XLanguage.Minutes(SaivoryKnife.GhostLifespan), BottleOSpirits.GhostSlotIncrease);

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var Aequu2 = player.GetModPlayer<Aequu2Player>();
        Aequu2.ghostLifespan.Flat += 3600;
        Aequu2.ghostSlotsMax++;
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
