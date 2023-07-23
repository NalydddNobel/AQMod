using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Hexoplasm;

public class HexxedBar : ModItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DevelopmentFeatures;
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Ectoplasm;
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.SpectreBar);
        //Item.createTile = ModContent.TileType<HexxedBarPlaced>();
        //Item.placeStyle = 0;
    }

    public override Color? GetAlpha(Color lightColor) {
        return null;
    }

    public override void AddRecipes() {
        CreateRecipe(2)
            .AddIngredient(ItemID.ChlorophyteBar, 2)
            .AddIngredient<Hexoplasm>()
            .AddTile(TileID.AdamantiteForge)
            .TryRegisterAfter(ItemID.SpectreBar);
    }
}