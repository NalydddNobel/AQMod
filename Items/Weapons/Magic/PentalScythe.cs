using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class PentalScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new DynamicInventoryGlowmask(GlowID.PentalScythe, getGlowmaskColor), item.type);
        }

        private static Color getGlowmaskColor() => Color.Lerp(new Color(188, 175, 135, 0), new Color(144, 130, 111, 0), ((float)Math.Sin(Main.GlobalTime * 1.1f) + 1f) / 2f);

        public override void SetDefaults()
        {
            item.damage = 30;
            item.magic = true;
            item.useTime = 40;
            item.useAnimation = 40;
            item.width = 24;
            item.height = 24;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<FlamingScythe>();
            item.shootSpeed = 24.11f;
            item.mana = 24;
            item.autoReuse = true;
            item.UseSound = SoundID.Item8;
            item.value = AQItem.EnergyWeaponValue;
            item.knockBack = 2.43f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float rot = new Vector2(speedX, speedY).ToRotation();
            float rotOffset = MathHelper.PiOver2 / 5f;
            float speed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            for (int i = 0; i < 5; i++)
            {
                Vector2 velo = new Vector2(1f, 0f).RotatedBy(rot + (i - 2) * rotOffset);
                Projectile.NewProjectile(position + velo * 10f, velo, type, damage, knockBack, player.whoAmI, 0f, speed * 0.01f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemonScythe);
            recipe.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 10);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class FlamingScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.tileCollide = false;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 3;
        }

        public override void AI()
        {
            projectile.rotation += 1.15f;
            projectile.ai[0]++;
            if (projectile.ai[0] < 120)
                projectile.ai[1] += projectile.ai[0] / 480;
            float turnSpeed = projectile.ai[0] / 3000f;
            float speed = projectile.ai[1];
            float rotation = projectile.velocity.ToRotation();
            int targetIndex = AQNPC.FindClosest(projectile.Center, 1000f);
            if (targetIndex != -1)
            {
                NPC target = Main.npc[targetIndex];
                Vector2 difference = target.Center - projectile.Center;
                turnSpeed = MathHelper.Lerp(turnSpeed, turnSpeed + (1000f - (difference.Length() * 0.11f - projectile.ai[1] * difference.Length())) * 0.00035f, turnSpeed * 1.35f);
                rotation = rotation.AngleLerp((target.Center - projectile.Center).ToRotation(), MathHelper.Clamp(turnSpeed, 0f, 1f));
            }
            projectile.velocity = new Vector2(speed, 0f).RotatedBy(rotation);
        }

        public override void PostAI()
        {
            Lighting.AddLight(projectile.Center, new Vector3(0.5f, 0.3f, 0.05f));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = Main.projectileTexture[projectile.type].Size() / 2;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] + new Vector2(projectile.width / 2f, projectile.height / 2f) - Main.screenPosition, null, new Color(60, 60, 60, 0), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, new Color(240, 240, 240, 130), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            }
        }
    }
}