using Aequus.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Misc {
    public class MoonlunaHatProj : ModProjectile
    {
        public TrailRenderer prim;

        public virtual int Max => 10;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 360;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 60;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            switch ((int)Projectile.ai[1])
            {
                case 1:
                    return Color.AliceBlue.UseA(0);
                case 2:
                    return new Color(200, 80, 111, 0);
                case 3:
                    return new Color(200, 175, 75, 0);
                case 4:
                    return new Color(80, 70, 255, 0);
                case 5:
                    return new Color(80, 180, 255, 0);
                case 6:
                    return new Color(180, 80, 255, 0);
                case 7:
                    return new Color(120, 150, 255, 0);
            }
            return Color.White.UseA(0);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[1] == 0)
            {
                Projectile.ai[0] = -1f;
                int highestTimeLeft = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].type == Type && Projectile.Distance(Main.projectile[i].Center) < 240f && Main.projectile[i].timeLeft > highestTimeLeft)
                    {
                        Projectile.ai[0] = i;
                        highestTimeLeft = Main.projectile[i].timeLeft;
                    }
                }
                Projectile.ai[1] = Main.rand.Next(7) + 1;
                Projectile.localAI[1] = Main.rand.NextFloat(-0.2f, 0.2f);
                Projectile.netUpdate = true;
            }
            if ((int)Projectile.ai[0] > -1)
            {
                if (!Main.projectile[(int)Projectile.ai[0]].active || Main.projectile[(int)Projectile.ai[0]].type != Type)
                {
                    Projectile.ai[0] = -1f;
                }
            }
            int count = Main.player[Projectile.owner].ownedProjectileCounts[Type];
            if (Projectile.timeLeft < 30)
            {
                int amt = Math.Max(count - Max, 1);
                Projectile.timeLeft -= amt - 1;
                Projectile.alpha += 25 * amt;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                }
                Projectile.scale = Projectile.Opacity;
            }
            else
            {
                if (count > Max)
                {
                    CheckOldest();
                }
                if (Projectile.alpha > 0)
                {
                    Projectile.scale = Projectile.Opacity;
                    Projectile.alpha -= 20;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }
            }
        }

        public void CheckOldest()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == Type && Main.projectile[i].timeLeft < Projectile.timeLeft)
                {
                    return;
                }
            }
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 30);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if ((int)Projectile.ai[0] > -1)
            {
                if (prim == null)
                {
                    prim = new TrailRenderer(TrailTextures.Trail[0].Value, TrailRenderer.DefaultPass,
                        (p) => new Vector2(8f), (p) =>
                        Color.Lerp(Projectile.GetAlpha(Color.White), Main.projectile[(int)Projectile.ai[0]].GetAlpha(Color.White), p) * 0.75f * (Projectile.Opacity * Main.projectile[(int)Projectile.ai[0]].Opacity), obeyReversedGravity: false, worldTrail: false);
                }
                var endCoords = Main.projectile[(int)Projectile.ai[0]].Center;
                var coords = new Vector2[] { Projectile.Center - Main.screenPosition, (Projectile.Center + endCoords) / 2f - Main.screenPosition, endCoords - Main.screenPosition, };
                prim.Draw(coords);
            }
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);

            var color = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale * 0.33f + Projectile.localAI[1] * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, color * 0.4f, Projectile.rotation, origin, Projectile.scale * 0.6f + Projectile.localAI[1] * Projectile.scale, SpriteEffects.None, 0);
            if (Aequus.HQ)
            {
                foreach (var v in Helper.CircularVector(4, MathHelper.PiOver4))
                {
                    Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition + v * 4f * (Projectile.scale + Projectile.localAI[1]), frame, color * 0.1f, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale * 0.5f + Projectile.localAI[1] * Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return false;
        }
    }
}