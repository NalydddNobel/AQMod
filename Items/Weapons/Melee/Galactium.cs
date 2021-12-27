using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Galactium : ModItem
    {
        public override void SetStaticDefaults()
        {
            AQItem.CreativeMode.SingleItem(this);
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 6f;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 38;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = AQItem.Rarities.OmegaStariteRare;
            Item.shoot = ModContent.ProjectileType<Projectiles.Melee.Galactium>();
            Item.shootSpeed = 25f;
            Item.value = AQItem.Prices.OmegaStariteWeaponValue;
            Item.autoReuse = true;
            Item.scale = 1.1f;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 15);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 240);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 1.5);
            position = new Vector2(player.position.X, player.position.Y - 800f);
            velocity = Vector2.Normalize(Main.MouseWorld - position) * velocity.Length();
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + new Vector2((float)Math.Sin(position.X - position.Y + Main.MouseWorld.X - Main.MouseWorld.Y) * 10f, 0f), Vector2.Normalize(Main.MouseWorld + new Vector2((float)Math.Sin(position.X + position.Y + Main.MouseWorld.X + Main.MouseWorld.Y) * 100f, 0f) - position) * velocity.Length(), type, damage, knockback, player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item9, position);
            return true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StariteBlade>()
                .AddIngredient(ItemID.Starfury)
                .AddIngredient<CosmicEnergy>(5)
                .AddIngredient(ItemID.SoulofFlight, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}