using Aequus.Common.DataSets;
using Aequus.Common.Recipes;
using Aequus.Content.Items.Materials.OmniGem;

namespace Aequus.Content.Items.Tools.Consumable.OmniPaint;

public class OmniPaint : ModItem {
    public static readonly int CraftAmount = 25;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.RedPaint);
        Item.paint = PaintID.None;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(silver: 2);
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
    }

    public override void UpdateInventory(Player player) {
        if (Main.myPlayer != player.whoAmI) {
            return;
        }

        var ui = ModContent.GetInstance<OmniPaintUI>();
        var heldItem = player.HeldItemFixed();
        if (!Main.playerInventory && heldItem != null && !heldItem.IsAir && ItemSets.IsPaintbrush.Contains(heldItem.type)) {
            ui.Enabled = true;
        }
        Item.paint = ui.SelectedPaint;
        Item.paintCoating = ui.SelectedCoating;
    }

    public override void AddRecipes() {
        CreateRecipe(CraftAmount)
            .AddRecipeGroup(AequusRecipes.AnyPaints, CraftAmount)
            .AddIngredient<OmniGem>()
            .AddTile(TileID.DyeVat)
            .Register();
    }
}