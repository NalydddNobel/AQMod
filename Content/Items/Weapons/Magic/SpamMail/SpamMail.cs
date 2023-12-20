using Aequus.Content.Items.Material;
using Aequus.Content.Tiles.CraftingStations.TrashCompactor;

namespace Aequus.Content.Items.Weapons.Magic.SpamMail;
public class SpamMail : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<SpamMailProj>(), 4, 14f, hasAutoReuse: true);
        Item.SetWeaponValues(4, 1f);
        Item.useAnimation *= 5;
        Item.reuseDelay = 15;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<CompressedTrash>(12)
            .AddIngredient(ItemID.FallenStar, 2)
            .AddTile<TrashCompactor>()
            .Register();
    }
}