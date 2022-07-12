using Aequus.Projectiles.Misc.Bobbers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.FishingRods
{
    public class Buzzer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);
            Item.fishingPole = 36;
            Item.shootSpeed = 16f;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 72);
            Item.shoot = ModContent.ProjectileType<BeeBobber>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.strongBees)
            {
                var offset = new Vector2(20f, 10f);
                int p = Projectile.NewProjectile(source, position + offset, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[p].ModProjectile<BeeBobber>().gotoPosition = Main.MouseWorld + offset;
                offset.X = -offset.X;
                p = Projectile.NewProjectile(source, position + offset, velocity, type, damage, knockback, player.whoAmI);
                Main.projectile[p].ModProjectile<BeeBobber>().gotoPosition = Main.MouseWorld + offset;
            }
            return true;
        }
    }
}