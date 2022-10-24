using Aequus.Graphics;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class HardwoodProj : ModProjectile
    {
        private const int goOutTime = 15;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            if (Projectile.localAI[1] >= 0)
            {
                if ((int)Projectile.localAI[1] == 0)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
                    Projectile.netUpdate = true;
                }
                if ((int)Projectile.localAI[1] < 45)
                {
                    Projectile.position -= Projectile.velocity * (1.2f - Projectile.localAI[1] / 45f * 0.2f);
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, newColor: new Color(10, 80, 255, 128) * Projectile.Opacity, Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.velocity *= 0.1f;
                    d.velocity += Vector2.Normalize(Projectile.velocity);
                }
                else if ((int)Projectile.localAI[1] > 80 && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.netUpdate = true;
                    Projectile.localAI[1] = -2f;
                    SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                    Collision.HitTiles(Projectile.position, Vector2.Zero, Projectile.width, Projectile.height);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        float distance = Projectile.Distance(Main.LocalPlayer.Center);
                        if (distance < 1000f)
                        {
                            EffectsSystem.Shake.Set(12f * (1f - distance / 1000f), 0.9f);
                        }
                    }
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 0.001f;
                }
                else if (Projectile.velocity.Length() < 12f)
                {
                    Projectile.velocity *= 1.05f;
                }
                Projectile.localAI[1]++;
            }
            Projectile.frame = 1;
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = (int)(Projectile.Center.X / 16f);
                Projectile.ai[1] = (int)(Projectile.Center.Y / 16f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            for (int i = 0; i < 30; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position - Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(90f), Projectile.width, Projectile.height, DustID.RichMahogany, Scale: Main.rand.NextFloat(0.6f, 2f));
                if (Main.rand.NextBool())
                {
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[1] < 45f && Projectile.localAI[1] >= 0f)
                return false;
            Projectile.GetDrawInfo(out var t, out var offset, out var frame, out var origin, out int _);
            var chainFrame = frame;
            chainFrame.Y = 0;
            var n = Vector2.Normalize(Projectile.velocity);
            AequusHelpers.DrawFramedChain(t, chainFrame, Projectile.Center, new Vector2(Projectile.ai[0] * 16f + 8f, Projectile.ai[1] * 16f + 8f) - n * origin.Y * 2f, Main.screenPosition);
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition + n * 54f, frame, AequusHelpers.GetColor(Projectile.Center), Projectile.rotation - MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}
