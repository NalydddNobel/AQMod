using AQMod.Assets;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Support
{
    public class IronShield : ModProjectile
    {
        public float maxPower;
        public float power;
        public bool baton;

        public static IronShield SpawnIronShield(Vector2 position, int damage, float knockback, Player player)
        {
            int type = ModContent.ProjectileType<IronShield>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type)
                    Main.projectile[i].Kill();
            }
            int p = Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return (IronShield)Main.projectile[p].modProjectile;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.timeLeft = 3600;
        }

        public bool UpdatePlayers()
        {
            bool buffedAnyPlayer = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var p = Main.player[i];
                if (p.dead || !p.active)
                    continue;
                var difference = p.Center - projectile.Center;
                float playerPower = power;
                if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, p.position, p.width, p.height))
                    playerPower /= 2f;
                if (difference.Length() < playerPower)
                {
                    p.AddBuff(BuffID.Ironskin, (int)(power * 2));
                    buffedAnyPlayer = true;
                    if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer == i)
                    {
                        var d75 = difference / 75f;
                        for (int j = 0; j < 75; j++)
                        {
                            int d = Dust.NewDust(projectile.Center + d75 * j + new Vector2(-6f, -6f), 12, 12, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(190, 255, 100, 0));
                            Main.dust[d].velocity *= 0.1f;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = Main.rand.NextFloat(0.75f, 2f);
                        }
                    }
                    else
                    {
                        var d20 = difference / 20f;
                        for (int j = 0; j < 20; j++)
                        {
                            int d = Dust.NewDust(projectile.Center + d20 * j, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(190, 255, 100, 0));
                            Main.dust[d].velocity *= 0.1f;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].scale = Main.rand.NextFloat(0.5f, 1.5f);
                        }
                    }
                }
            }
            return buffedAnyPlayer;
        }

        public override void AI()
        {
            if (Main.player[projectile.owner].dead && projectile.timeLeft > 120)
                projectile.timeLeft = 120;
            if (projectile.timeLeft < 120)
            {
                power = maxPower * (projectile.timeLeft / 120f);
                power += 2f;
            }
            if ((int)projectile.ai[0] == -1)
            {
                for (int i = 0; i < 5; i++)
                {
                    var pos = new Vector2(power, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                    if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.Center + pos, 2, 2))
                        pos /= 2f;
                    int d = Dust.NewDust(projectile.Center + pos, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(240, 255, 100, 0));
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                }
                if (baton)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float f = Main.rand.Next(20, (int)power);
                        float rot = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                        var pos2 = new Vector2(f, 0f).RotatedBy(rot);
                        if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.Center + new Vector2(power, 0f).RotatedBy(rot), 2, 2))
                            pos2 /= 2f;
                        int d2 = Dust.NewDust(projectile.Center + pos2, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(240, 255, 100, 0));
                        Main.dust[d2].noGravity = true;
                        Main.dust[d2].scale = Main.rand.NextFloat(0.8f, 2f);
                        var diff = projectile.Center - Main.dust[d2].position;
                        Main.dust[d2].velocity = Vector2.Normalize(diff) * (diff.Length() / 16f);
                    }
                }
                else
                {
                    float f = Main.rand.Next(20, (int)power);
                    float rot = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                    var pos2 = new Vector2(f, 0f).RotatedBy(rot);
                    if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.Center + new Vector2(power, 0f).RotatedBy(rot), 2, 2))
                        pos2 /= 2f;
                    int d2 = Dust.NewDust(projectile.Center + pos2, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(240, 255, 100, 0));
                    Main.dust[d2].noGravity = true;
                    Main.dust[d2].scale = Main.rand.NextFloat(0.5f, 1.5f);
                    var diff = projectile.Center - Main.dust[d2].position;
                    Main.dust[d2].velocity = Vector2.Normalize(diff) * (diff.Length() / 16f);
                }
                projectile.ai[1]++;
                if (projectile.ai[1] > power / 2f)
                {
                    projectile.ai[1] = 0f;
                    if (UpdatePlayers())
                        Main.PlaySound(SoundID.Item8);
                }
                return;
                projectile.frameCounter++;
                if (projectile.frameCounter > 4)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame >= 11)
                        projectile.frame = 7;
                }
                return;
            }
            else if ((int)projectile.ai[0] == 0)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    var m = Main.MouseWorld;
                    projectile.ai[0] = (int)m.X;
                    projectile.ai[1] = (int)m.Y;
                    projectile.netUpdate = true;
                }
                else
                {
                    return;
                }
            }
            var center = projectile.Center;
            var gotoPosition = new Vector2(projectile.ai[0], projectile.ai[1]);
            var difference = gotoPosition - center;
            float length = difference.Length();
            float off = (float)Math.Sin((projectile.position.X + projectile.position.Y) / 32f) * projectile.width;
            if (length < 120f)
                off *= length / 120f;
            Dust.NewDust(projectile.Center + new Vector2(off, 0f).RotatedBy(projectile.velocity.ToRotation() + MathHelper.PiOver2), 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(240, 255, 100, 0));
            if (length > 2f)
            {
                float speed;
                if (baton)
                {
                    speed = length / 16f;
                    if (speed < 1.5f)
                    {
                        speed = 1.5f;
                    }
                    else if (speed > 15f)
                    {
                        speed = 20f;
                    }
                }
                else
                {
                    speed = length / 32f;
                    if (speed < 1.5f)
                    {
                        speed = 1.5f;
                    }
                    else if (speed > 10f)
                    {
                        speed = 16f;
                    }
                }
                projectile.velocity = Vector2.Normalize(difference) * speed;
            }
            else
            {
                projectile.Center = gotoPosition;
                projectile.velocity = new Vector2(0f, 0f);
                projectile.ai[0] = -1f;
                projectile.ai[1] = maxPower;
                power = maxPower;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if ((int)projectile.ai[0] == -1)
            {
                var texture = this.GetTexture();
                var frame = projectile.ProjFrame(padding: 2);
                var color = projectile.GetAlpha(lightColor);
                var origin = frame.Size() / 2f;
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, new Color(255, 255, 255, 120), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return false;
        }
    }
}
