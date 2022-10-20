using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.ArcubusProjs
{
    public class ArcubusDamagingFloor : ModProjectile
    {
        public const int FramesX = 3;

        public List<Rectangle> DamageHitboxCache;

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
        }

        public override void AI()
        {
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
            if (Collision.SolidCollision(Projectile.position + new Vector2(Projectile.width/ 2f - 8f, Projectile.height / 2f), 16, 2))
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
                        if (Main.tile[i, j].IsSolid() && !Main.tile[i, j - 1].IsSolid() && Main.rand.NextBool(32 + Projectile.alpha / 2))
                        {
                            var d = Dust.NewDustPerfect(new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f), DustID.Torch, new Vector2(0f, Main.rand.NextFloat(-2.5f, -1f)), Scale: Main.rand.NextFloat(0.5f, 1.25f));
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
            int startX = (int)(Projectile.position.X / 16f);
            int startY = (int)(Projectile.position.Y / 16f);
            int endX = (int)((Projectile.position.X + Projectile.width) / 16f);
            int endY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    if (Main.tile[i, j].IsSolid())
                    {
                        DrawBurnSegment(i, j);
                    }
                }
            }
            return false;
        }
        public void DrawBurnSegment(int i, int j)
        {
            bool left = Main.tile[i - 1, j].IsFullySolid();
            bool right = Main.tile[i + 1, j].IsFullySolid();
            bool top = Main.tile[i, j - 1].IsFullySolid();
            bool bottom = Main.tile[i, j + 1].IsFullySolid();
            if (left && right && top && bottom)
                return;

            float distance = (new Vector2(i * 16f + 8f, j * 16f + 8f) - Projectile.Center).Length() / (Projectile.Size / 2f).Length();
            var color = Color.Lerp(new Color(255, 128, 50, 50), new Color(200, 48, 10, 0), AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f)) * (1f - distance) * Projectile.Opacity;
            var frame = new Rectangle(0, 0, 16, 16);
            if (left && right)
            {
                if (!top)
                {
                    DrawBurnPatch(top, i, j, 1, 0, color);
                }
            }
            else if (left && !top)
            {
                DrawBurnPatch(top, i, j, 2, 0, color);
            }
            else if (right && !top)
            {
                DrawBurnPatch(top, i, j, 0, 0, color);
            }
            else if (!top)
            {
                DrawBurnPatchNoTop(top, i, j, 0, 0, color);
                DrawBurnPatch(top, i, j, 2, 0, color);
            }
        }
        private void DrawBurnPatchToTile(int i, int j, Rectangle frame, Color color)
        {
            var drawCoords = new Vector2(i * 16f, j * 16f);
            if (Main.tile[i, j].IsHalfBlock)
            {
                drawCoords.Y += 8f;
                frame.Height -= 8;
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawCoords - Main.screenPosition, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        private void DrawBurnPatchNoTop(bool top, int i, int j, int frameX, int frameY, Color color)
        {
            var frame = Frame(frameX, frameY);
            frame.Y += 2;
            frame.Height -= 2;
            DrawBurnPatchToTile(i, j, frame, color);
        }
        private void DrawBurnPatch(bool top, int i, int j, int frameX, int frameY, Color color)
        {
            if (!top)
            {
                Main.EntitySpriteDraw(TextureCache.Bloom[0].Value, new Vector2(i * 16f, j * 16f - 16f) - Main.screenPosition, new Rectangle(TextureCache.Bloom[0].Value.Width / 2, 0, 1,
                    TextureCache.Bloom[0].Value.Height / 2), color.UseA(0) * 0.33f, 0f, Vector2.Zero, new Vector2(16f, 32f / TextureCache.Bloom[0].Value.Height), SpriteEffects.None, 0);
            }
            DrawBurnPatchToTile(i, j, Frame(frameX, frameY), color);
        }
        private Rectangle Frame(int x, int y)
        {
            return new Rectangle(x * 18, y * 18, 16, 16);
        }
    }
}