using Aequus.Common.Items;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Weapons.Melee.Slice;

public class Slice : ModItem {
    public static System.Single ProjectilePiercingPenalty { get; set; } = 0.5f;
    public static System.Int32 ProjectileDebuffDuration { get; set; } = 480;
    public static System.Int32 SwordDebuffDuration { get; set; } = 1000;

    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<SliceProj>(30);
        Item.shootSpeed = 15f;
        Item.SetWeaponValues(38, 4.5f, 6);
        Item.width = 20;
        Item.height = 20;
        Item.autoReuse = true;
        Item.rare = ItemCommons.Rarity.SpaceStormLoot;
        Item.value = ItemCommons.Price.SpaceStormLoot;
        Item.scale = 0.9f;
    }

    public override System.Boolean? UseItem(Player player) {
        return null;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(120);
    }

    public override System.Boolean MeleePrefix() {
        return true;
    }

    public override System.Boolean AltFunctionUse(Player player) {
        return false;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.CobaltSword)
            .AddIngredient(ItemID.FrostCore)
            //.AddIngredient<FrozenTear>(20)
            .AddTile(TileID.Anvils)
            .Register()
            .Clone().ReplaceItem(ItemID.CobaltSword, ItemID.PalladiumSword).Register();
    }

    public override System.Boolean Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, System.Int32 type, System.Int32 damage, System.Single knockback) {
        return true;
    }
}