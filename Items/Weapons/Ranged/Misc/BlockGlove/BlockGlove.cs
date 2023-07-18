using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Misc.BlockGlove;

public class BlockGlove : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.AleThrowingGlove);
        Item.useStyle = ItemUseStyleID.Swing;
        Item.damage = 12;
        Item.useTime = 17;
        Item.useAnimation = 17;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<CarpenterWeaponProj>();
        Item.useAmmo = ItemID.DirtBlock;
    }

    public override void UpdateInventory(Player player) {
        player.Aequus().hasBlockGlove = true;
    }

    public override bool? CanChooseAmmo(Item ammo, Player player) {
        return ammo.createTile >= TileID.Dirt && !Main.tileFrameImportant[ammo.createTile] && !ammo.IsACoin && ammo.tileWand < 0 ? true : null;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai0: new Item(source.AmmoItemIdUsed).createTile);
        return false;
    }
}