using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged.Dart
{
    public class Sprinkler : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 8;
            item.ranged = true;
            item.damage = 30;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.value = AQItem.EnergyWeaponValue;
            item.useAmmo = AmmoID.Dart;
            item.shoot = ProjectileID.Seed;
            item.shootSpeed = 28f;
            item.UseSound = SoundID.Item65;
            item.noMelee = true;
            item.rare = ItemRarityID.Lime;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int dustAmount = Main.rand.Next(10, 15);
            Vector2 normalizedVelocity = Vector2.Normalize(new Vector2(speedX, speedY));
            Vector2 spawnPosition = position + normalizedVelocity * 46f + new Vector2(0, -4f);
            for (int i = 0; i < dustAmount; i++)
            {
                int d = Dust.NewDust(spawnPosition, 10, 10, 33);
                Vector2 velocity = normalizedVelocity.RotatedBy(Main.rand.NextFloat(-0.314f, 0.314f));
                Main.dust[d].velocity.X = velocity.X * Main.rand.NextFloat(6f, 12f);
                Main.dust[d].velocity.Y = velocity.Y * Main.rand.NextFloat(6f, 12f);
            }
            for (int i = 0; i < dustAmount * 1.5; i++)
            {
                int d = Dust.NewDust(spawnPosition, 10, 10, 15);
                Vector2 velocity = normalizedVelocity.RotatedBy(Main.rand.NextFloat(-0.157f, 0.157f));
                Main.dust[d].velocity.X = velocity.X * Main.rand.NextFloat(3f, 6f);
                Main.dust[d].velocity.Y = velocity.Y * Main.rand.NextFloat(3f, 6f);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StarPhish>());
            recipe.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 10);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}