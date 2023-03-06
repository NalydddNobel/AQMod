using Aequus.Content;
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
    public class MendshroomProj : ModProjectile
    {
        public override string Texture => $"{Aequus.VanillaTexture}Projectile_{ProjectileID.SporeTrap}";

        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.timeLeft = 480;
            Projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(50, 200, 100, 128);
        }

        public override void AI()
        {
            if (Projectile.alpha < 50)
            {
                int grabRange = 200;
                if (Main.player[Projectile.owner].lifeMagnet)
                {
                    grabRange += Item.lifeGrabRange;
                }
                if (Main.player[Projectile.owner].treasureMagnet)
                {
                    grabRange += Item.treasureGrabRange;
                }

                grabRange /= 2;
                int closestPlr = -1;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead)
                    {
                        var plrHitbox = Main.player[i].Hitbox;
                        if (Projectile.Hitbox.Intersects(plrHitbox))
                        {
                            Main.player[i].Heal(Math.Max(Main.player[i].statLifeMax / 25, 1));
                            SoundEngine.PlaySound(SoundID.Grab.WithVolume(0.6f), Projectile.Center);
                            SoundEngine.PlaySound(SoundID.Item4.WithVolume(0.15f).WithPitchOffset(0.5f), Projectile.Center);
                            Projectile.Kill();
                            for (int k = 0; k < 40; k++)
                            {
                                if (Main.rand.NextBool(4))
                                {
                                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                                        ModContent.DustType<MendshroomDustSpore>(), Scale: Main.rand.NextFloat(0.5f, 1f));
                                    d.velocity *= 0.5f;
                                    d.velocity -= Projectile.velocity * 0.1f;
                                }
                                else
                                {
                                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                                        ModContent.DustType<MonoDust>(), newColor: new Color(20, 200, 150, 128) * Main.rand.NextFloat(0.5f, 1f), Scale: Main.rand.NextFloat(0.5f, 2f));
                                    d.velocity *= 0.5f;
                                    d.velocity -= Projectile.velocity * 0.75f * Main.rand.NextFloat();
                                }
                            }
                            return;
                        }
                        else if (Projectile.timeLeft < 500)
                        {
                            float distance = Projectile.Distance(Main.player[i].Center);
                            if (distance < grabRange)
                            {
                                grabRange = (int)distance;
                                closestPlr = i;
                            }
                        }
                    }
                }

                if (closestPlr != -1)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[closestPlr].Center - Projectile.Center) * 20f, 0.2f);
                    Projectile.timeLeft = Math.Max(Projectile.timeLeft, 200);
                    Projectile.tileCollide = false;
                }

                if (Main.rand.NextBool(20))
                {
                    Dust d;
                    if (Main.rand.NextBool(4))
                    {
                        d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                            ModContent.DustType<MendshroomDustSpore>(), Scale: Main.rand.NextFloat(0.5f, 1f));
                        d.velocity = -Projectile.velocity * 0.1f;
                        d.velocity += (d.position - Projectile.Center) / 16f;
                    }
                    else
                    {
                        d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                            ModContent.DustType<MonoDust>(), newColor: new Color(20, 200, 150, 128) * Main.rand.NextFloat(0.5f, 1f), Scale: Main.rand.NextFloat(0.5f, 2f));
                        d.velocity = -Projectile.velocity * 0.75f * Main.rand.NextFloat();
                        d.velocity += (d.position - Projectile.Center) / 4f;
                    }
                }
            }

            Projectile.scale = 0.75f + Projectile.alpha / 255f;
            Projectile.rotation += Projectile.scale * 0.1f;
            if ((int)Projectile.ai[0] == 1 || Projectile.timeLeft < 30)
            {
                Projectile.alpha += 2;
                if (Projectile.timeLeft < 30)
                    Projectile.alpha += 5;
                if (Projectile.alpha > 50)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else if ((int)Projectile.ai[0] == 0)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha > 50)
                {
                    Projectile.alpha -= 9;
                }
                if (Projectile.alpha <= 0)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].Aequus().cMendshroom, null);
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(AequusTextures.Bloom0, Projectile.position + off - Main.screenPosition, null, Projectile.GetAlpha(lightColor).UseA(0) * Projectile.Opacity, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, Projectile.scale * 0.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(AequusTextures.Bloom0, Projectile.position + off - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * 0.35f * Projectile.Opacity, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            return false;
        }
    }
}