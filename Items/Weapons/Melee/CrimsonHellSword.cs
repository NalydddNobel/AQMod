using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Effects.Batchers;
using AQMod.Effects.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class CrimsonHellSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlayData(AQUtils.GetPath(this) + "_Glow", new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 26;
            item.useTime = 48;
            item.useAnimation = 24;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shoot = ModContent.ProjectileType<BurnterizerProjectile>();
            item.shootSpeed = 12f;
            item.scale = 1.35f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.GetAlphaDemonSiegeWeapon(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int size = 16;
            var off = new Vector2(-size / 2f);
            var velo = new Vector2(speedX, speedY) * 0.1f;
            for (int i = 0; i < 25; i++)
            {
                int dustType = Main.rand.NextBool(5) ? 31 : DustID.Fire;
                var p = position + new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) + off;
                int d = Dust.NewDust(p, size, size, dustType);
                Main.dust[d].velocity = Vector2.Lerp(Main.dust[d].velocity, velo, 0.3f);
                if (dustType != 31)
                    Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
                Main.dust[d].noGravity = true;
            }
            Main.PlaySound(SoundID.DD2_BetsyFireballShot, position);
            return true;
        }
    }

    public class BurnterizerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.penetrate = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 80);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.Kill();
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            Main.dust[d].velocity = -projectile.velocity * 0.1f;
            Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
            Main.dust[d].noGravity = true;
            if (Main.rand.NextBool(4))
            {
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31);
                Main.dust[d].velocity = -projectile.velocity * 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
                Main.dust[d].noGravity = true;
            }
            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.5f, 0.1f));
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            if (Main.myPlayer == projectile.owner && AQMod.TonsofScreenShakes)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center);
                if (distance < 700)
                {
                    GameScreenManager.AddEffect(new ScreenShake(8, AQMod.MultIntensity((int)(700f - distance) / 32)));
                }
            }
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BurnterizerExplosion>(), 30, projectile.knockBack, projectile.owner);
            Vector2 position = projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;
            var bvelo = -projectile.velocity * 0.375f;
            for (int i = 0; i < 6; i++)
            {
                Gore.NewGore(Main.projectile[p].Center, bvelo * 0.2f, 61 + Main.rand.Next(3));
            }
            for (int i = 0; i < 24; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, 31);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 60; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var drawPosition = projectile.Center - Main.screenPosition;
            var color = projectile.GetAlpha(lightColor);
            var origin = new Vector2(texture.Width / 2f, 10f);
            var drawData = new DrawData(texture, drawPosition, null, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
            var batcher = new GeneralEntityBatcher(Main.spriteBatch);
            bool resetBatch = false;
            if (AQConfigClient.Instance.OutlineShader)
            {
                resetBatch = true;
                batcher.StartShaderBatch();
                float intensity = (float)Math.Sin(Main.GlobalTime * 10f) + 1.5f;
                var effect = GameShaders.Misc["AQMod:OutlineColor"];
                effect.UseColor(new Vector3(1f, 0.5f * intensity, 0.1f * intensity));
                effect.Apply(drawData);
            }
            drawData.Draw(Main.spriteBatch);
            if (resetBatch)
                batcher.StartBatch();
            drawData.scale *= 1.25f;
            drawData.color *= 0.25f;
            drawData.Draw(Main.spriteBatch);
            return false;
        }
    }

    public class BurnterizerExplosion : ModProjectile
    {
        public override string Texture => "AQMod/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.timeLeft = 2;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
            {
                projectile.active = false;
            }
            projectile.ai[0]++;
        }
    }
}