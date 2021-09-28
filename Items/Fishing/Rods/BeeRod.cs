using AQMod.Items.Energies;
using AQMod.Projectiles.Bobbers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fishing.Rods
{
    public class BeeRod : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodFishingPole);
            item.fishingPole = 36;
            item.shootSpeed = 16f;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<BeeBobber>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.strongBees)
            {
                var offset = new Vector2(20f, 10f);
                int p = Projectile.NewProjectile(position + offset, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                ((BeeBobber)Main.projectile[p].modProjectile).gotoPosition = Main.MouseWorld + offset;
                offset.X = -offset.X;
                p = Projectile.NewProjectile(position + offset, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                ((BeeBobber)Main.projectile[p].modProjectile).gotoPosition = Main.MouseWorld + offset;
            }
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.BeeWax, 6);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>());
            r.AddIngredient(ItemID.Hive, 20);
            r.AddIngredient(ItemID.BottledHoney);
            r.AddTile(ModContent.TileType<Tiles.FishingCraftingStation>());
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}