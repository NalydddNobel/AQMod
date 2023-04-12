using Aequus;
using Aequus.Common.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.Crabson.Projectiles
{
    public class CrabsonBubble : ModProjectile
    {
        private float _start;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.6f, 1f));
        }

        public override void AI()
        {
            int target = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            Projectile.width = 16;
            Projectile.height = 16;
            if ((int)Projectile.ai[0] != 0f)
            {
                if (_start == 0f)
                {
                    _start = Projectile.ai[0];
                }
                Projectile.ai[0]--;
                if ((int)Projectile.ai[0] == 0)
                {
                    Projectile.position.Y -= Projectile.height / 2f;
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Main.expertMode ? 10f : 6f);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item85.WithVolume(0.7f), Projectile.Center);
                    }
                }
                if (Projectile.scale < 1f)
                {
                    Projectile.scale += 0.05f;
                    if (Projectile.scale > 1f)
                    {
                        Projectile.scale = 1f;
                    }
                }
            }
            else
            {
                if (Projectile.scale < 1f)
                {
                    Projectile.scale = 1f;
                }
                Projectile.ai[1]++;
                if (!Main.getGoodWorld && Projectile.ai[1] > 30f)
                {
                    Projectile.tileCollide = true;
                }
                float amt = Main.expertMode ? 0.05f : 0.025f;
                if (Main.getGoodWorld)
                {
                    amt *= 2.5f;
                }
                Projectile.velocity.X += Main.rand.NextFloat(-amt, amt);
            }
            if (!Main.getGoodWorld || Main.player[target].active && !Main.player[target].dead && (Projectile.velocity.Y > 0f ? Projectile.position.Y < Main.player[target].position.Y : Projectile.position.Y > Main.player[target].position.Y))
            {
                Projectile.tileCollide = false;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 30);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 8;
            Projectile.tileCollide = false;
            Projectile.ai[0] = -1;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] <= 0f || Projectile.alpha > 0 || LegacyEffects.ProjsBehindTiles.RenderingNow)
            {
                var texture = TextureAssets.Projectile[Projectile.type].Value;
                var drawColor = Projectile.GetAlpha(lightColor);
                var frame = texture.Frame();
                var offset = new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;
                int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
                var trailColor = new Color(10, 80, 160, 0);
                if (Main.getGoodWorld)
                {
                    drawColor *= 0.5f;
                    trailColor *= 0.5f;
                }
                var origin = frame.Size() / 2f;
                int trailRemove = Math.Min((int)Projectile.ai[0], 0);
                for (int i = 0; i < trailLength + trailRemove; i++)
                {
                    float progress = 1f - 1f / trailLength * (i - trailRemove);
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset, frame, trailColor * progress, 0f, origin, Projectile.scale * progress, SpriteEffects.None, 0f);
                }
                var drawPos = Projectile.position + offset;
                drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);
                if (Projectile.ai[0] > 0f && !Main.getGoodWorld)
                {
                    var bloom = AequusTextures.Bloom0;
                    var bloomFrame = new Rectangle(0, 0, bloom.Width, bloom.Height / 2);
                    var bloomOrigin = new Vector2(bloomFrame.Width / 2f, bloomFrame.Height);
                    var bloomColor = new Color(25, 25, 255, 0);
                    float alpha = 0f;
                    if (_start != 0f && Projectile.ai[0] > _start - 24f)
                    {
                        alpha = (Projectile.ai[0] - (_start - 24f)) / 24f;
                    }
                    bloomColor *= alpha;
                    Main.spriteBatch.Draw(bloom, drawPos, bloomFrame, bloomColor, Projectile.rotation + MathHelper.PiOver2, bloomOrigin, new Vector2(0.15f, 4f * alpha), SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(bloom, drawPos, bloomFrame, bloomColor * 0.6f, Projectile.rotation + MathHelper.PiOver2, bloomOrigin, new Vector2(0.2f, 6f * alpha), SpriteEffects.None, 0f);
                }
                if (trailRemove == 0)
                {
                    Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                LegacyEffects.ProjsBehindTiles.Add(Projectile.whoAmI);
            }
            return false;
        }
    }
}