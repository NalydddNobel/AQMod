using AQMod.Assets;
using AQMod.Effects.ScreenEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class BreadsoulHealing : ModProjectile
    {
        public static void SpawnCluster(Player owner, Vector2 position, float size, int amount, int hpHeal = 30)
        {
            int buffTime = hpHeal * 5 / amount;
            var normal = Vector2.Normalize(position - owner.Center);
            int type = ModContent.ProjectileType<BreadsoulHealing>();
            float minSpeed = size * 0.05f;
            float maxSpeed = size * 0.1f;
            float off = size * 0.75f;
            int dustAmount = (int)(12 * AQConfigClient.c_EffectQuality);
            float dRot = MathHelper.TwoPi / dustAmount;
            for (int j = 0; j < dustAmount * 3; j++)
            {
                int d = Dust.NewDust(position, 2, 2, 180);
                Main.dust[d].velocity = new Vector2(10f, 0f).RotatedBy(MathHelper.TwoPi / (dustAmount * 3) * j);
                Main.dust[d].scale = 1.65f;
                Main.dust[d].alpha = 180;
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < amount; i++)
            {
                var normal2 = normal.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2));
                var heartPos = position + normal2 * off;
                int p = Projectile.NewProjectile(heartPos, normal2 * Main.rand.NextFloat(minSpeed, maxSpeed), type, -1, 0f, owner.whoAmI, 0f, buffTime);
                Main.projectile[p].timeLeft += Main.rand.Next(-40, 40);
                for (int j = 0; j < dustAmount; j++)
                {
                    int d = Dust.NewDust(heartPos, 2, 2, 180);
                    Main.dust[d].velocity = new Vector2(10f, 0f).RotatedBy(dRot * j);
                    Main.dust[d].scale = 1.25f;
                    Main.dust[d].alpha = 20;
                    Main.dust[d].noGravity = true;
                }
            }
            if (AQConfigClient.c_TonsofScreenShakes)
            {
                ScreenShakeManager.AddShake(new BasicScreenShake(16, 8));
            }
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.netImportant = true;
            projectile.timeLeft = 300;
            projectile.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        // ai[1] is buff time
        public override void AI()
        {
            int nearestPlayer = Player.FindClosest(projectile.position, projectile.width, projectile.height);
            var player = Main.player[nearestPlayer];
            if (Main.player[nearestPlayer].dead || !Main.player[nearestPlayer].active)
            {
                if (projectile.timeLeft > 30)
                    projectile.timeLeft = 30;
                return;
            }
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var difference = player.Center - projectile.Center;
            float distance = difference.Length();
            if (distance < player.width * aQPlayer.grabReachMult)
            {
                HealPlayer(p: nearestPlayer);
            }
            else if (distance < 180f * aQPlayer.grabReachMult)
            {
                if (projectile.timeLeft < 30)
                    projectile.timeLeft = 30;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * 24f, 0.125f);
                int d2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 180);
                Main.dust[d2].velocity *= 0.1f;
                Main.dust[d2].velocity += -projectile.velocity * 0.35f;
                Main.dust[d2].scale = Main.rand.NextFloat(1.45f, 1.75f);
                Main.dust[d2].alpha = Main.rand.Next(10);
                Main.dust[d2].noGravity = true;
                d2 = Dust.NewDust(projectile.position + projectile.Size / 4f, projectile.width / 2, projectile.height / 2, 180);
                Main.dust[d2].velocity *= 0.1f;
                Main.dust[d2].velocity += -projectile.velocity * 0.1f;
                Main.dust[d2].scale = Main.rand.NextFloat(0.9f, 1.1f);
                Main.dust[d2].alpha = Main.rand.Next(50, 125);
                Main.dust[d2].noGravity = true;
            }
            else
            {
                projectile.velocity *= 0.96f;
            }
            if (Main.rand.NextBool(7))
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 180);
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].velocity += -projectile.velocity * 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 1.1f);
                Main.dust[d].alpha = Main.rand.Next(50, 125);
                Main.dust[d].noGravity = true;
            }
        }

        private void HealPlayer(int p)
        {
            var player = Main.player[p];
            Main.PlaySound(SoundID.DD2_DarkMageCastHeal, player.Center);
            player.AddBuff(ModContent.BuffType<Buffs.SpectreHealing>(), (int)projectile.ai[1]);
            projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (AQConfigClient.c_EffectQuality <= 1f)
                return true;
            var texture = Main.projectileTexture[projectile.type];
            var origin = texture.Size() / 2f;
            var clr = projectile.GetAlpha(lightColor);
            var pos = projectile.Center;
            Main.spriteBatch.Draw(texture, pos - Main.screenPosition, null, clr, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            var spotlight = AQTextures.Lights[LightTex.Spotlight30x30];
            float lightScaleMult = 1f - projectile.velocity.Length() / 8f;
            if (lightScaleMult < 0.05f)
            {
                return false;
            }
            lightScaleMult += (float)Math.Sin(projectile.timeLeft) * 0.04f;
            Main.spriteBatch.Draw(texture, pos - Main.screenPosition, null, clr, projectile.rotation, origin, new Vector2(projectile.scale * 0.3f, projectile.scale) * lightScaleMult, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, pos - Main.screenPosition, null, clr, projectile.rotation, origin, new Vector2(projectile.scale, projectile.scale * 0.3f) * lightScaleMult, SpriteEffects.None, 0f);
            return false;
        }
    }
}