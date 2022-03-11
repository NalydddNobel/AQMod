using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public sealed class FlameblasterFire : ModProjectile
    {
        public override string Texture => Tex.None;

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 18;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 50;
            projectile.alpha = 255;
            projectile.hide = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            bool canScale = projectile.width < 120;
            if (projectile.wet)
            {
                projectile.Kill();
            }
            else if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.velocity *= 0.85f + Main.rand.NextFloat(0.1f);
                projectile.velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
                if (projectile.width > 20)
                {
                    AQProjectile.Scale(projectile, -8);
                }
                canScale = false;
                if (projectile.velocity.Length() < 1f)
                {
                    projectile.Kill();
                }
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 60;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            else
            {
                if (canScale && Main.rand.NextBool())
                {
                    AQProjectile.Scale(projectile, 4);
                    projectile.netUpdate = true;
                }
                projectile.velocity.Y -= 0.005f;
                float scaling = Math.Min(projectile.width / 40f, 1f);
                for (int i = 0; i < (int)(5 * scaling); i++)
                {
                    if (projectile.timeLeft > 4 || Main.rand.NextBool(9))
                    {

                        var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(150, 75, 18, 50));
                        d.velocity *= 0.6f;
                        d.velocity += projectile.velocity * 0.1f;
                        d.noGravity = true;
                        d.scale *= Main.rand.NextFloat(1.25f, 3.15f) * scaling;
                    }
                }
                var smaller = new Rectangle((int)projectile.position.X + projectile.width / 2 - projectile.width / 4, (int)projectile.position.Y + projectile.height / 2 - projectile.height / 4, projectile.width / 2, projectile.height / 2);
                for (int i = 0; i < (int)(5 * scaling); i++)
                {
                    var d = Dust.NewDustDirect(smaller.TopLeft(), smaller.Width, smaller.Height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(255, 120, 50, 20));
                    d.velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * 0.1f;
                    d.noGravity = true;
                    d.scale *= Main.rand.NextFloat(1f, 2.15f) * scaling;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.width);
            writer.Write(projectile.height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.width = reader.ReadInt32();
            projectile.height = reader.ReadInt32();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 1200);
        }
    }
}