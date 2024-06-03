using Aequus.Core.CodeGeneration;
using Aequus.DataSets;
using System.Linq;

namespace Aequus.Old.Content.Necromancy.Equipment.Accessories;

[Gen.AequusPlayer_ResetField<int>("ghostShadowDash")]
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
        foreach (int demoniteBar in ItemTypeVariantDataSet.DemoniteBar.Where(i => i.ValidEntry)) {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 100)
                .AddIngredient(demoniteBar, 3)
                .AddTile(TileID.DemonAltar)
                .Register()
                .SortBeforeFirstRecipesOf(ItemID.MasterNinjaGear);
        }
    }
}