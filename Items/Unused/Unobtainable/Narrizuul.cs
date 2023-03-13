using Aequus.Content.ItemRarities;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused.Unobtainable
{
    public class Narrizuul : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(777, 7.77f, 3);
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<NarrizuulProj>(), 7, 27.77f, true);
            Item.useAnimation = 14;
            Item.mana = 7;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ModContent.RarityType<DevItemRarity>();
            Item.value = Item.sellPrice(gold: 50);
            Item.UseSound = SoundID.Item1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 * 0.5f), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4 * 0.5f), type, damage, knockback, player.whoAmI);
            return true;
        }
    }
}