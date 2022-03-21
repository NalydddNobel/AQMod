using AQMod.Effects;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class CrabsonBubble : ModProjectile
    {
        private float _start;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.scale = 0.1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, AQUtils.Wave(Main.GlobalTime * 5f, 0.6f, 1f));
        }

        public override void AI()
        {
            int target = Player.FindClosest(projectile.position, projectile.width, projectile.height);
            projectile.width = 16;
            projectile.height = 16;
            if ((int)projectile.ai[0] != 0f)
            {
                if (_start == 0f)
                {
                    _start = projectile.ai[0];
                }
                projectile.ai[0]--;
                if ((int)projectile.ai[0] == 0)
                {
                    projectile.position.Y -= projectile.height / 2f;
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * (Main.expertMode ? 10f : 6f);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundID.Item85.Play(projectile.Center, 0.7f);
                    }
                }
                if (projectile.scale < 1f)
                {
                    projectile.scale += 0.05f;
                    if (projectile.scale > 1f)
                    {
                        projectile.scale = 1f;
                    }
                }
            }
            else
            {
                if (projectile.scale < 1f)
                {
                    projectile.scale = 1f;
                }
                projectile.ai[1]++;
                if (projectile.ai[1] > 30f)
                {
                    projectile.tileCollide = true;
                }
                float amt = Main.expertMode ? 0.05f : 0.025f;
                projectile.velocity.X += Main.rand.NextFloat(-amt, amt);
            }
            if (Main.player[target].active && !Main.player[target].dead && (projectile.velocity.Y > 0f ? projectile.position.Y < Main.player[target].position.Y : projectile.position.Y > Main.player[target].position.Y))
            {
                projectile.tileCollide = false;
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 30);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.timeLeft = 8;
            projectile.tileCollide = false;
            projectile.ai[0] = -1;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] <= 0f || projectile.alpha > 0 || DrawHelper.ProjsBehindTiles.drawingNow)
            {
                var texture = Main.projectileTexture[projectile.type];
                var drawColor = projectile.GetAlpha(lightColor);
                var frame = texture.Frame();
                var offset = new Vector2(projectile.width / 2, projectile.height / 2) - Main.screenPosition;
                int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
                var trailColor = new Color(10, 80, 160, 0);
                var origin = frame.Size() / 2f;
                int trailRemove = Math.Min((int)projectile.ai[0], 0);
                for (int i = 0; i < trailLength + trailRemove; i++)
                {
                    float progress = 1f - 1f / trailLength * (i - trailRemove);
                    spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, trailColor * progress, 0f, origin, projectile.scale * progress, SpriteEffects.None, 0f);
                }
                var drawPos = projectile.position + offset;
                drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);
                if (projectile.ai[0] > 0f)
                {
                    var bloom = AQMod.Texture("Assets/EffectTextures/Bloom");
                    var bloomFrame = new Rectangle(0, 0, bloom.Width, bloom.Height / 2);
                    var bloomOrigin = new Vector2(bloomFrame.Width / 2f, bloomFrame.Height);
                    var bloomColor = new Color(25, 25, 255, 0);
                    float alpha = 0f;
                    if (_start != 0f && projectile.ai[0] > _start - 24f)
                    {
                        alpha = (projectile.ai[0] - (_start - 24f)) / 24f;
                    }
                    bloomColor *= alpha;
                    spriteBatch.Draw(bloom, drawPos, bloomFrame, bloomColor, projectile.rotation + MathHelper.PiOver2, bloomOrigin, new Vector2(0.15f, 4f * alpha), SpriteEffects.None, 0f);
                    spriteBatch.Draw(bloom, drawPos, bloomFrame, bloomColor * 0.6f, projectile.rotation + MathHelper.PiOver2, bloomOrigin, new Vector2(0.2f, 6f * alpha), SpriteEffects.None, 0f);
                }
                if (trailRemove == 0)
                {
                    spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                DrawHelper.ProjsBehindTiles.Add(projectile.whoAmI);
            }
            return false;
        }
    }
}