using Aequus.Common;
using Aequus.Old.Content.Items.Materials;

namespace Aequus.Old.Content.Items.Weapons.Melee.SuperStarSword;

public class SuperStarSword : ModItem {
    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<SuperStarSwordProj>(32);
        Item.SetWeaponValues(26, 4.5f, 6);
        Item.width = 20;
        Item.height = 20;
        Item.scale = 1.25f;
        Item.autoReuse = true;
        Item.rare = Commons.Rare.GlimmerLoot;
        Item.value = Commons.Cost.GlimmerLoot;
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        damage /= 2;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<StariteMaterial>(16)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}