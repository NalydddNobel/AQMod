using Aequus.Projectiles.Misc;
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
        public override string Texture => Aequus.AssetsPath + "Explosion1";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
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
            Projectile.timeLeft = 100;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 150, 20, 100);
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
            Projectile.frame = 3;
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
                    if (Projectile.timeLeft > 4 || Main.rand.NextBool(9))
                    {
                        var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                        d.velocity *= 0.75f;
                        d.velocity += -Projectile.velocity * 0.33f;
                        d.noGravity = true;
                        d.scale *= Main.rand.NextFloat(0.5f, 2.15f) * scaling;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 1200);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            var c = Projectile.GetAlpha(lightColor);
            var bloomColor = c * 2;
            c = new Color(c.R - Projectile.alpha, c.G - Projectile.alpha, c.B - Projectile.alpha, c.A - Projectile.alpha);
            bloomColor = new Color(bloomColor.R - Projectile.alpha, bloomColor.G - Projectile.alpha, bloomColor.B - Projectile.alpha, bloomColor.A - Projectile.alpha);
            Main.spriteBatch.Draw(TextureCache.Bloom[0].Value, Projectile.position + offset - Main.screenPosition, null, bloomColor, 0f, TextureCache.Bloom[0].Value.Size() / 2f, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class FlameblasterWind : PumpinatorProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 90;
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.extraUpdates = 8;
        }
    }
}