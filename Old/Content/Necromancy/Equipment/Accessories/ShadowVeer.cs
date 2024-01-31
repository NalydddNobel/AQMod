namespace Aequus.Old.Content.Necromancy.Equipment.Accessories;

public class ShadowVeer : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().ghostShadowDash++;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.Cobweb, 100)
            .AddIngredient(ItemID.DemoniteBar, 10)
            .AddTile(TileID.DemonAltar)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.MasterNinjaGear)

            .Clone()
            .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.MasterNinjaGear);
    }
}