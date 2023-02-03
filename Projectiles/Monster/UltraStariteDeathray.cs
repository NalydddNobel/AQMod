using Aequus.Graphics.Primitives;
using Aequus.NPCs.Boss.OmegaStarite;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Projectiles.Monster
{
    public class UltraStariteDeathray : EnemyAttachedProjBase
    {
        public const float DEATHRAY_SIZE = 60f;
        public const float DEATHRAY_LENGTH = 1000f;
        public const int DEATHRAY_KILL_TIME = 25;

        public override string Texture => Aequus.BlankTexture;

        public TrailRenderer prim;
        public TrailRenderer smokePrim;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft *= 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.hide = true;
        }

        protected override bool CheckAttachmentConditions(NPC npc)
        {
            return npc.ai[0] == UltraStarite.STATE_DEATHRAY && npc.ModNPC is UltraStarite;
        }

        public override void AI()
        {
            base.AI();
            Projectile.Opacity = Projectile.timeLeft / (float)DEATHRAY_KILL_TIME;
        }

        protected override void AIAttached(NPC npc)
        {
            Projectile.velocity *= 0.5f;
            Projectile.rotation = npc.rotation - MathHelper.PiOver2;
            if (npc.ai[0] == UltraStarite.STATE_DEATHRAY)
                Projectile.timeLeft = DEATHRAY_KILL_TIME;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly && target.life > 5 ? false : null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            var offset = normal * OmegaStarite.DIAMETER;
            var end = Projectile.Center + offset + normal * DEATHRAY_LENGTH;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + offset, end, DEATHRAY_SIZE * Projectile.scale, ref _);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = new Color(10, 200, 80, 0);
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var n = Projectile.rotation.ToRotationVector2();
            var arr = new Vector2[] {
                    drawPos,
                    drawPos + n * Main.screenWidth * Projectile.direction,
                    drawPos + n * Main.screenWidth * 2f * Projectile.direction, };
            if (prim == null)
            {
                prim = new TrailRenderer(TextureCache.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(70f), (p) => Color.BlueViolet.UseA(0) * 1.4f * (float)Math.Pow(1f - p, 2f) * 0.4f * Projectile.Opacity, obeyReversedGravity: false, worldTrail: false);
            }

            if (smokePrim == null)
            {
                smokePrim = new ForceCoordTrailRenderer(TextureCache.Trail[3].Value, TrailRenderer.DefaultPass, (p) => new Vector2(40f), (p) => Color.Blue.UseR(60).UseG(160).UseA(0) * (1f - p) * 0.8f * Projectile.Opacity, obeyReversedGravity: false, worldTrail: false)
                {
                    coord1 = 0f,
                    coord2 = 1f
                };
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(arr);
            }
            var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f);
            int amount = (int)(5 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
            var initialArr = new Vector2[amount];
            var center = Projectile.Center;

            initialArr[0] = arr[0];
            for (int i = 1; i < amount; i++)
            {
                initialArr[i] = drawPos + new Vector2(60f / amount * i * -Projectile.direction, 0f);
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(initialArr);
            }
            // funny prim shenanigans
            //prim.Draw(initialArr);
            //smokePrim.Draw(initialArr, -Main.GlobalTimeWrappedHourly * 2f, 4f);
            prim.Draw(arr);
            smokePrim.Draw(arr, -Main.GlobalTimeWrappedHourly, 2f);

            prim.Draw(arr);
            smokePrim.Draw(arr, -Main.GlobalTimeWrappedHourly, 2f);

            var spotlight = TextureCache.Bloom[2].Value;
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.4f, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * (Projectile.height / 32f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.5f * (Projectile.height / 32f), SpriteEffects.None, 0f);
            return false;
        }

        public float GetLaserScale()
        {
            return Projectile.timeLeft <= DEATHRAY_KILL_TIME ? 1f / DEATHRAY_KILL_TIME * Projectile.timeLeft * Projectile.scale : Projectile.scale;
        }
    }
}