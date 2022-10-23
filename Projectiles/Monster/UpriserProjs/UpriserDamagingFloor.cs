using Aequus.Graphics.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.UpriserProjs
{
    public class UpriserDamagingFloor : ModProjectile
    {
        public const int FramesX = 3;

        public override string Texture => Aequus.AssetsPath + "Pixel";

        public List<(Vector4, Func<Color>)> drawCache;
        public List<Rectangle> DamageHitboxCache;
        private bool subscribedToRender;
        private float subscribingOpacity;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            DamageHitboxCache = new List<Rectangle>();
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if ((int)Projectile.localAI[0] == 0)
            {
                var spawnLoc = new Vector2(Projectile.position.X + Projectile.width / 2f, Projectile.position.Y + Projectile.height);
                for (int i = 0; i < 10; i++)
                {
                    var d = Dust.NewDustPerfect(spawnLoc, DustID.Smoke, new Vector2(Main.rand.NextFloat(-12f, 12f), Main.rand.NextFloat(-10f, 0f)), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    d.fadeIn = d.scale + 0.25f;
                    d.noGravity = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustPerfect(spawnLoc, DustID.Torch, new Vector2(Main.rand.NextFloat(-12f, 12f), Main.rand.NextFloat(-10f, 0f)), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    d.fadeIn = d.scale + 0.25f;
                    d.noGravity = true;
                }
                Projectile.localAI[0] = 1f;
            }
            if (Projectile.timeLeft < 20)
            {
                Projectile.alpha += 12;
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            if (Collision.SolidCollision(Projectile.position + new Vector2(Projectile.width / 2f - 8f, Projectile.height / 2f), 16, 2))
            {
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity.Y += 0.3f;
            }
            Projectile.ai[0]++;
            int startX = (int)(Projectile.position.X / 16f);
            int startY = (int)(Projectile.position.Y / 16f);
            int endX = (int)((Projectile.position.X + Projectile.width) / 16f);
            int endY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            if (DamageHitboxCache.Count == 0 || Projectile.ai[0] > 10f)
            {
                DamageHitboxCache?.Clear();

                var p = new List<Point>();
                for (int i = startX; i <= endX; i++)
                {
                    var r = new Rectangle(i, 0, 1, 1);
                    for (int j = startY; j <= endY; j++)
                    {
                        if (Main.tile[i, j].IsSolid())
                        {
                            if (r.Y == 0)
                                r.Y = j;
                            else
                                r.Height++;
                        }
                        else if (r.Y != 0)
                        {
                            DamageHitboxCache.Add(new Rectangle(r.X * 16 - 2, r.Y * 16 - 2, r.Width * 16 + 4, r.Height * 16 + 4));
                            r.Y = 0;
                        }
                    }
                    if (r.Y != 0)
                    {
                        DamageHitboxCache.Add(new Rectangle(r.X * 16 - 2, r.Y * 16 - 2, r.Width * 16 + 4, r.Height * 16 + 4));
                        r.Y = 0;
                    }
                }
            }
            if (Projectile.alpha < 50)
            {
                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        if (Main.tile[i, j].IsSolid() && !Main.tile[i, j - 1].IsSolid() && Main.rand.NextBool(32 + Projectile.alpha / 2 + (int)(32f * (1f - subscribingOpacity))))
                        {
                            var d = Dust.NewDustPerfect(new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f), DustID.Torch, new Vector2(0f, Main.rand.NextFloat(-2.5f, -1f)), Scale: Main.rand.NextFloat(0.5f, 1.25f) * subscribingOpacity);
                            d.noGravity = true;
                            d.fadeIn = d.scale + Main.rand.NextFloat(1f);
                        }
                    }
                }
            }
            Projectile.ai[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (var r in DamageHitboxCache)
            {
                if (targetHitbox.Intersects(r))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!subscribedToRender)
            {
                if (drawCache == null)
                    drawCache = new List<(Vector4, Func<Color>)>();
                SpecialTileRenderer.AdjustTileTarget.Add((flag) =>
                {
                    subscribedToRender = false;
                    if (!flag)
                        return;

                    int startX = (int)(Projectile.position.X / 16f);
                    int startY = (int)(Projectile.position.Y / 16f);
                    int endX = (int)((Projectile.position.X + Projectile.width) / 16f);
                    int endY = (int)((Projectile.position.Y + Projectile.height) / 16f);
                    var drawColor = Color.Lerp(Color.Yellow, Color.Red, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0.6f, 0.8f));
                    var drawOffset = AequusHelpers.TileDrawOffset;
                    drawCache.Clear();
                    for (int i = startX; i <= endX; i++)
                    {
                        for (int j = startY; j <= endY; j++)
                        {
                            if (Main.tile[i, j].IsSolid() && !Main.tile[i, j - 1].IsFullySolid())
                            {
                                int pixelStartX = i * 16 - (int)Main.screenPosition.X + (int)drawOffset.X;
                                int pixelStartY = j * 16 - (int)Main.screenPosition.Y + (int)drawOffset.Y;
                                float distanceOpacity = 1f - (float)Math.Pow(Vector2.Distance(new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition, Projectile.Center - Main.screenPosition) / (Projectile.Size / 2f).Length(), 2f);
                                for (int k = 0; k < 16; k += 2)
                                {
                                    float drawnDown = 1f;
                                    float drawDownSubtractor = 0.1f + (1f - Projectile.Opacity);
                                    int max = 18;
                                    bool drawnAura = false;
                                    for (int l = -2; l < max; l += 2)
                                    {
                                        float drawnDownCache = drawnDown;
                                        if (!SpecialTileRenderer.TileTargetColors.InBounds(pixelStartX + k, pixelStartY + l))
                                            break;
                                        var clr = SpecialTileRenderer.TileTargetColors[pixelStartX + k, pixelStartY + l];
                                        if (clr.A == 0)
                                            continue;

                                        if (!drawnAura)
                                        {
                                            drawnAura = true;
                                            float auraOpacity = (AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2f + i * 0.1f, 0.4f, 1f) + AequusHelpers.Wave(-Main.GlobalTimeWrappedHourly + i * 0.05f, 0.4f, 1f)) / 2f;
                                            int auraMax = (int)(16 * Projectile.Opacity * auraOpacity * distanceOpacity) / 2;
                                            for (int n = 0; n < auraMax * 2; n++)
                                            {
                                                if (!SpecialTileRenderer.TileTargetColors.InBounds(pixelStartX + k, pixelStartY + l - n))
                                                {
                                                    break;
                                                }
                                                int nCache = n;
                                                drawCache.Add((new Vector4(pixelStartX + k + (int)Main.screenPosition.X - (int)AequusHelpers.TileDrawOffset.X, pixelStartY + l - n + (int)Main.screenPosition.Y - (int)AequusHelpers.TileDrawOffset.Y, 0f, 0f), 
                                                    () => drawColor * ((float)Math.Pow((1f - 1f / auraMax * ((nCache - 1) / 2)) * Projectile.Opacity * subscribingOpacity, 2f) / 2f * distanceOpacity)));
                                            }
                                        }
                                        drawCache.Add((new Vector4(pixelStartX + k + (int)Main.screenPosition.X - (int)AequusHelpers.TileDrawOffset.X, pixelStartY + l + (int)Main.screenPosition.Y - (int)AequusHelpers.TileDrawOffset.Y, 0f, 0f),
                                            () => drawColor.HueAdd(-(1f - drawnDownCache) * 0.1f).UseA(100) * (float)Math.Pow(drawnDownCache * Projectile.Opacity * subscribingOpacity, 4f) * distanceOpacity));

                                        drawnDown -= drawDownSubtractor;
                                        if (drawnDown <= 0f)
                                            break;
                                        max++;
                                    }
                                }
                            }
                        }
                    }
                    subscribingOpacity += 0.05f + subscribingOpacity / 16f;
                });
            }
            if (subscribingOpacity > 0f)
            {
                subscribingOpacity += 0.01f;
            }
            if (subscribingOpacity > 1f)
                subscribingOpacity = 1f;
            var t = TextureAssets.Projectile[Type].Value;
            var scale = new Vector2(2f);
            foreach (var d in drawCache)
            {
                Main.spriteBatch.Draw(t, new Vector2(d.Item1.X, d.Item1.Y) - Main.screenPosition, null, d.Item2(), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            subscribedToRender = true;
            return false;
        }
    }
}