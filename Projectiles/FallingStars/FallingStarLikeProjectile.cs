using AQMod.Common.Graphics;
using AQMod.Content.World.FallingStars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.FallingStars
{
    public abstract class FallingStarLikeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            //projectile.aiStyle = 5;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 50;
            //projectile.light = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, lightColor.A - projectile.alpha);
        }

        public override void AI()
        {
            if (Main.dayTime && projectile.damage >= 500)
            {
                projectile.Kill();
            }
            if (projectile.ai[1] == 0f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[1] = 1f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[1] != 0f)
            {
                projectile.tileCollide = true;
            }
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                Main.PlaySound(SoundID.Item9, projectile.position);
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
            }
            projectile.alpha += (int)(25f * projectile.localAI[0]);
            if (projectile.alpha > 200)
            {
                projectile.alpha = 200;
                projectile.localAI[0] = -1f;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
                projectile.localAI[0] = 1f;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;
            AQGraphics.SetCullPadding();
            if (AQGraphics.Cull_WorldPosition(projectile.getRect()) && Main.rand.Next(6) == 0)
            {
                Gore.NewGore(projectile.position, projectile.velocity * 0.2f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17));
            }
            projectile.light = 0.9f;
            if (Main.rand.Next(20) == 0)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
            }

            Lighting.AddLight(projectile.Center, ProjLight * projectile.light);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        protected virtual int DustAmountOnKill => 7;
        protected virtual int StarsAmountOnKill => 7;
        protected virtual int DroppedItem => ItemID.FallenStar;
        protected virtual Vector3 ProjLight => new Vector3(0.9f, 0.8f, 0.1f);
        protected virtual Color SparkleDustColor => Color.CornflowerBlue;
        protected virtual Color SparkleDustColorGold => Color.Gold;
        protected virtual Color TrailColor => Color.Blue * 0.2f;
        protected virtual Color TrailColorWhite => Color.White * 0.5f;

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer && !projectile.noDropItem && DroppedItem != ItemID.None && projectile.damage > 500)
            {
                int item = Item.NewItem(projectile.getRect(), DroppedItem);
                if (item >= 0)
                {
                    Main.item[item].GetGlobalItem<GlimmerItem>().shimmering = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                    }
                }
            }

            Main.PlaySound(SoundID.Item10, projectile.position);

            for (int i = 0; i < DustAmountOnKill; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, SparkleDustColor).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, SparkleDustColorGold).noGravity = true;
            }
            AQGraphics.SetCullPadding();
            if (AQGraphics.Cull_WorldPosition(projectile.getRect()) && Main.rand.Next(6) == 0)
            {
                for (int i = 0; i < StarsAmountOnKill; i++)
                {
                    Gore.NewGore(projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * projectile.velocity.Length(), Utils.SelectRandom<int>(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var projectileTexture = Main.projectileTexture[projectile.type];
            var frame = new Rectangle(0, 0, projectileTexture.Width, projectileTexture.Height);
            var origin = frame.Size() / 2f;
            var alpha = projectile.GetAlpha(lightColor);
            var trailThing = ModContent.GetTexture("AQMod/Projectiles/FallingStars/Trail");
            var trailFrame = trailThing.Frame();
            var trailOrigin = new Vector2((float)trailFrame.Width / 2f, 10f);
            var gfxOff = new Vector2(0f, projectile.gfxOffY);
            var spinningpoint = new Vector2(0f, -10f);
            float visualEffectsTimer = Main.GlobalTime;
            var vector36 = projectile.Center + projectile.velocity;
            var trailColor = TrailColor;
            var trailColorWhite = TrailColorWhite;
            trailColorWhite.A = 0;
            float num189 = 0f;
            var color45 = trailColor;
            color45.A = 0;
            var color46 = trailColor;
            color46.A = 0;
            var color47 = trailColor;
            color47.A = 0;
            Main.spriteBatch.Draw(trailThing, vector36 - Main.screenPosition + gfxOff + spinningpoint.RotatedBy((float)Math.PI * 2f * visualEffectsTimer), trailFrame, color45, projectile.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 1.5f + num189, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(trailThing, vector36 - Main.screenPosition + gfxOff + spinningpoint.RotatedBy((float)Math.PI * 2f * visualEffectsTimer + (float)Math.PI * 2f / 3f), trailFrame, color46, projectile.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 1.1f + num189, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(trailThing, vector36 - Main.screenPosition + gfxOff + spinningpoint.RotatedBy((float)Math.PI * 2f * visualEffectsTimer + 4.18879032f), trailFrame, color47, projectile.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 1.3f + num189, SpriteEffects.None, 0);
            var vector37 = projectile.Center - projectile.velocity * 0.5f;
            for (float num190 = 0f; num190 < 1f; num190 += 0.5f)
            {
                float num191 = visualEffectsTimer % 0.5f / 0.5f;
                num191 = (num191 + num190) % 1f;
                float num192 = num191 * 2f;
                if (num192 > 1f)
                {
                    num192 = 2f - num192;
                }
                Main.spriteBatch.Draw(trailThing, vector37 - Main.screenPosition + gfxOff, trailFrame, trailColorWhite * num192, projectile.velocity.ToRotation() + (float)Math.PI / 2f, trailOrigin, 0.3f + num191 * 0.5f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(projectileTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), frame, alpha, projectile.rotation, origin, projectile.scale + 0.1f, projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
    }
}