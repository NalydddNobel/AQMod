using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Ranged
{
    public class Sprinkler : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 8;
            item.ranged = true;
            item.damage = 20;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3.5f;
            item.autoReuse = true;
            item.value = AQItem.Prices.PostMechsEnergyWeaponValue;
            item.useAmmo = AmmoID.Dart;
            item.shoot = ProjectileID.Seed;
            item.shootSpeed = 24f;
            item.UseSound = SoundID.Item65;
            item.noMelee = true;
            item.rare = AQItem.Rarities.GaleStreamsRare;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.myPlayer != player.whoAmI || AQConfigClient.c_EffectQuality < 0.3f)
                return true;
            int dustAmount = (int)(Main.rand.Next(5, 10) * AQConfigClient.c_EffectQuality);
            var normalizedVelocity = Vector2.Normalize(new Vector2(speedX, speedY));
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
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StarPhish>());
            recipe.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            recipe.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 2);
            recipe.AddIngredient(ItemID.SoulofFlight, 12);
            recipe.AddIngredient(ItemID.SoulofNight, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}