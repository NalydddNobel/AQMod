using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class GebulbaStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 16;
            item.magic = true;
            item.useTime = 42;
            item.useAnimation = 42;
            item.width = 40;
            item.height = 40;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<GelBulb>();
            item.shootSpeed = 10f;
            item.mana = 5;
            item.autoReuse = true;
            item.UseSound = SoundID.Item85;
            item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 20);
            recipe.AddRecipeGroup("IronBar", 4);
            recipe.AddTile(TileID.Solidifier);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class GelBulb : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.penetrate = 5;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.25f;
        }

        private void CollisionEffects(Vector2 velocity)
        {
            Vector2 spawnPos = projectile.position + velocity;
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Boing").WithPitchVariance(projectile.velocity.Length() / 16f), projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(spawnPos, projectile.width, projectile.height, DustID.t_Slime);
                Main.dust[d].color = AQNPC.BlueSlimeColor;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool doEffects = false;
            if (oldVelocity.X != projectile.velocity.X && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                projectile.position.X += projectile.velocity.X * 0.9f;
                projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (oldVelocity.Y != projectile.velocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                projectile.position.Y += projectile.velocity.Y * 0.9f;
                projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            if (doEffects)
            {
                CollisionEffects(projectile.velocity);
            }
            else
            {
                projectile.velocity *= 0.95f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item85.WithPitchVariance(2f), projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime);
                Main.dust[d].color = AQNPC.BlueSlimeColor;
            }
        }
    }
}