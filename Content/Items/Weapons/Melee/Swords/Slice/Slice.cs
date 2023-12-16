using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.Swords.Slice;

public class Slice : ModItem {
    public static float ProjectilePiercingPenalty = 0.5f;
    public static int ProjectileDebuffDuration = 480;
    public static int SwordDebuffDuration = 1000;

    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<SliceProj>(30);
        Item.shootSpeed = 15f;
        Item.SetWeaponValues(38, 4.5f, 6);
        Item.width = 20;
        Item.height = 20;
        Item.autoReuse = true;
        Item.rare = ItemDefaults.Rarity.SpaceStormLoot;
        Item.value = ItemDefaults.Price.SpaceStormLoot;
        Item.scale = 0.9f;
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(120);
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override bool AltFunctionUse(Player player) {
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

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        return true;
    }
}