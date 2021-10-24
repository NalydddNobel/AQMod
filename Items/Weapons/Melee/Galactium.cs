using AQMod.Common;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Galactium : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 42;
            item.melee = true;
            item.knockBack = 6f;
            item.width = 32;
            item.height = 32;
            item.useTime = 38;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.GlimmeringStar>();
            item.shootSpeed = 15f;
            item.value = AQItem.OmegaStariteWeaponValue;
            item.autoReuse = true;
            item.scale = 1.1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            damage = (int)(damage * 1.5);
            position = new Vector2(player.position.X, player.position.Y - 800f);
            var newVelocity = Vector2.Normalize(Main.MouseWorld - position) * (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            speedX = newVelocity.X;
            speedY = newVelocity.Y;
            Projectile.NewProjectile(position, Vector2.Normalize(Main.MouseWorld + new Vector2((float)Math.Sin(position.X + position.Y + Main.MouseWorld.X + Main.MouseWorld.Y) * 100f, 0f) - position) * (float)Math.Sqrt(speedX * speedX + speedY * speedY), type, damage, knockBack, player.whoAmI);
            position += new Vector2((float)Math.Sin(position.X - position.Y + Main.MouseWorld.X - Main.MouseWorld.Y) * 10f, 0f);
            Main.PlaySound(SoundID.Item9, position);
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<StariteBlade>());
            r.AddIngredient(ItemID.Starfury);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 5);
            r.AddIngredient(ItemID.SoulofFlight, 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}