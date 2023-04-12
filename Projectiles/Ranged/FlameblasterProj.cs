using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class FlameblasterProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(210, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }

        public override void AI()
        {
            bool canScale = Projectile.width < 120;
            Projectile.scale = Projectile.width / 80f;
            if (Projectile.rotation == 0f)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            Projectile.rotation += 0.025f;
            //Projectile.frame = 3;
            if (Projectile.numUpdates == -1)
                Projectile.position += Main.player[Projectile.owner].velocity * 0.8f;
            if (Projectile.wet)
            {
                Projectile.Kill();
            }
            else if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.velocity *= 0.85f + Main.rand.NextFloat(0.1f);
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
                if (Projectile.width > 20)
                {
                    AequusProjectile.Scale(Projectile, -8);
                }
                canScale = false;
                if (Projectile.velocity.Length() < 1f)
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.alpha > 0 && Projectile.timeLeft > 30)
            {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            else
            {
                if (Projectile.timeLeft < 30)
                {
                    Projectile.alpha += 255 / 30;
                }
                if (canScale && Main.rand.NextBool())
                {
                    AequusProjectile.Scale(Projectile, 4);
                    Projectile.netUpdate = true;
                }
                Projectile.velocity.Y -= 0.005f;
                float scaling = Math.Min(Projectile.width / 40f, 1f);
                for (int i = 0; i < (int)(2 * scaling); i++)
                {
                    if (Main.rand.NextBool(16 + Projectile.alpha))
                    {
                        var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.NextFloat(1.5f, 2.15f) * scaling * 2f);
                        d.velocity *= 0.75f;
                        d.velocity += -Projectile.velocity * 0.33f;
                        d.velocity *= 0.5f;
                        d.noGravity = true;
                    }
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.width);
            writer.Write(Projectile.height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.width = reader.ReadInt32();
            Projectile.height = reader.ReadInt32();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
            Projectile.damage = (int)(Projectile.damage * 0.75f);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}