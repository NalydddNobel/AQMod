using Aequus.Common.Items;
using Aequus.Items.Materials.Glimmer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.SuperStarSword;

public class SuperStarSword : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAequusSword<SuperStarSwordProj>(32);
        Item.SetWeaponValues(22, 4.5f, 6);
        Item.width = 20;
        Item.height = 20;
        Item.scale = 1.25f;
        Item.rare = ItemDefaults.RarityGlimmer;
        Item.autoReuse = true;
        Item.value = ItemDefaults.ValueGlimmer;
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
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