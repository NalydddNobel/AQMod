using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public sealed class SnowgraveProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 180;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.coldDamage = true;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 4;
        }

        public override void AI()
        {
            if ((int)projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.width / 6;
            }
            if (projectile.ai[0] > 0f)
                projectile.ai[0]--;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage -= (int)projectile.ai[0];
            damage = Math.Max(damage, 1);
            if (target.position.X + target.width / 2f < Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2f)
            {
                hitDirection = -1;
            }
            else
            {
                hitDirection = 1;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, crit ? 480 : 240);
            switch (target.type) 
            {
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                    {
                        projectile.ai[0] += 50f;
                    }
                    break;
            }
            var center = target.Center;
            float l = target.Size.Length() / 4f;
            float r = Main.rand.NextFloat(-(MathHelper.TwoPi / 12f), MathHelper.TwoPi / 12f);
            for (int i = 0; i < 12; i++)
            {
                var normal = Vector2.UnitX.RotatedBy(i * (MathHelper.TwoPi / 12f) + r);
                Dust.NewDustPerfect(center + normal * l, ModContent.DustType<MonoDust>(), normal * Main.rand.NextFloat(2.5f, 7.5f), 0, new Color(80, 120, 255, 128), Main.rand.NextFloat(0.9f, 1.3f));
            }
            if (crit)
            {
                r = Main.rand.NextFloat(-(MathHelper.TwoPi / 4f), MathHelper.TwoPi / 4f);
                for (int i = 0; i < 4; i++)
                {
                    var normal = Vector2.UnitX.RotatedBy(i * (MathHelper.TwoPi / 4f) + r);
                    Dust.NewDustPerfect(center + normal * l, ModContent.DustType<MonoSparkleDust>(), normal * Main.rand.NextFloat(7f, 10f), 0, new Color(120, 160, 255, 128), Main.rand.NextFloat(1.2f, 1.5f));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var center = projectile.Center - Main.screenPosition;
            var texture = Main.projectileTexture[projectile.type];
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            var color = projectile.GetAlpha(lightColor);
            spriteBatch.Draw(texture, center, frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, new Vector2(center.X + projectile.localAI[0] * i, center.Y), frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, new Vector2(center.X - projectile.localAI[0] * i, center.Y), frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}