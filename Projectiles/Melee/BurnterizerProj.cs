using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Effects.ScreenEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class BurnterizerProj : ModProjectile
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
            if (Main.myPlayer == projectile.owner && AQConfigClient.c_TonsofScreenShakes)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center);
                if (distance < 700)
                {
                    ScreenShakeManager.AddShake(new BasicScreenShake(8, AQGraphics.MultIntensity((int)(700f - distance) / 32)));
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
            if (!AQMod.LowQ)
            {
                Main.spriteBatch.End();
                BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);
                float intensity = (float)Math.Sin(Main.GlobalTime * 10f) + 1.5f;
                var effect = EffectCache.s_OutlineColor;
                effect.UseColor(new Vector3(1f, 0.5f * intensity, 0.1f * intensity));
                effect.Apply(drawData);
                drawData.Draw(Main.spriteBatch);
                Main.spriteBatch.End();
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                drawData.scale *= 1.25f;
                drawData.color *= 0.25f;
                drawData.Draw(Main.spriteBatch);
            }
            else
            {
                drawData.Draw(Main.spriteBatch);
            }
            return false;
        }
    }
}