using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class HyperCrystalProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            this.SetTrail(10);
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 150 * (1 + Projectile.extraUpdates);
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.frame = Main.rand.Next(4);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            var aequus = Projectile.Aequus();
            if (aequus.sourceProj != -1 && Main.projectile[aequus.sourceProj].active)
            {
                if (aequus.sourceProj == Main.player[Projectile.owner].heldProj)
                {
                    Projectile.velocity = Main.projectile[aequus.sourceProj].velocity;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    aequus.sourceProj = -1;
                    aequus.sourceProjIdentity = -1;
                    return;
                }
                Projectile.ai[1] += 0.055f;
                Projectile.localAI[0] = (float)Math.Cos(Projectile.ai[1]);
                Projectile.scale = 1f + Projectile.localAI[0] * 0.3f;
                Projectile.velocity = Main.projectile[aequus.sourceProj].velocity;
                Projectile.Center = Vector2.Lerp(Projectile.Center, 
                    Main.projectile[aequus.sourceProj].oldPosition + Main.projectile[aequus.sourceProj].Size / 2f + new Vector2(Math.Max(Main.projectile[aequus.sourceProj].width, 20) * (float)Math.Sin(Projectile.ai[1]), 0f).RotatedBy(Projectile.rotation), 0.05f + (255 - Projectile.alpha) / 2000f) - Projectile.velocity;
            }
            else if ((int)Projectile.ai[0] == 1)
            {
                float speed = Projectile.velocity.Length();
                if (speed != 4f)
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 4f;
                }
            }
            else if ((int)Projectile.ai[0] == 2)
            {
                Projectile.velocity *= 0.98f;
                Projectile.ai[1]++;
                Projectile.rotation += Projectile.ai[1] * 0.001f;
            }
            if ((Projectile.alpha == 255 || Projectile.alpha < 200) && Projectile.Distance(Main.player[Projectile.owner].Center) > 480f)
            {
                Projectile.Kill();
            }

            if (Projectile.alpha > 0 && Projectile.numUpdates == -1)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.localAI[0] < 0f)
            {
                behindProjectiles.Add(index);
                Projectile.hide = true;
            }
            else
            {
                Projectile.hide = false;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.alpha > 200)
            {
                return;
            }
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath.WithVolume(0.6f), Projectile.Center);
            for (int i = 0; i < 34; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 
                    newColor: Color.Lerp(new Color(100, 255, 255, 100), new Color(100, 255, 100, 100), Main.rand.NextFloat()), Scale: Main.rand.NextFloat(0.5f, 1.1f));
                d.velocity *= 0.5f;
                d.velocity += Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(15f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].Aequus().cHyperCrystal);
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);
            frame.Width /= 4;
            frame.X = frame.Width * Projectile.frame;
            frame.Y = 0;
            origin = frame.Size() / 2f;
            for (int i = 0; i < trailLength; i++)
            {
                var p = AequusHelpers.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, new Color(255, 255, 255, 100) * 0.35f * p * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, new Color(255, 255, 255, 255) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}