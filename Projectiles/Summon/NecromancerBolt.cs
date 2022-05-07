using Aequus.Buffs.Debuffs;
using Aequus.Graphics;
using Aequus.Graphics.Prims;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class NecromancerBolt : ModProjectile
    {
        protected PrimRenderer prim;
        protected float primScale;
        protected Color primColor;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.scale = 0.8f;
            Projectile.alpha = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 140, 255, 255 - Projectile.alpha);
        }

        public override void AI()
        {
            Projectile.damage = Math.Max(Projectile.damage, 1);
            if (Main.myPlayer == Projectile.owner)
            {
                var center = Projectile.Center;
                foreach (var v in AequusHelpers.CircularVector(3, Main.GlobalTimeWrappedHourly * 5f))
                {
                    Dust.NewDustPerfect(center + v * Projectile.width, ModContent.DustType<MonoDust>(), Projectile.velocity * -0.1f + Main.rand.NextVector2Unit() * 0.2f, Math.Min(Projectile.alpha * 4, 255), new Color(1, 20, 100, 100), 1.5f);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = 0;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            NecromancyDebuff.ApplyDebuff<NecromancyDebuff>(target, 600, Projectile.owner, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var batchData = new SpriteBatchData(Main.spriteBatch);

            Main.spriteBatch.End();
            batchData.BeginCustom(Main.spriteBatch, blendState: BlendState.Additive);

            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(10, 40, 255, 100), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            batchData.Begin(Main.spriteBatch);
            return false;
        }

        protected void DrawTrail(float waveSize = 8f, int maxLength = -1)
        {
            if (prim == null)
            {
                prim = new PrimRenderer(Images.Trail[0].Value, PrimRenderer.DefaultPass, (p) => new Vector2(primScale) * (1f - p), (p) => primColor * (1f - p));
            }

            int trailLength = maxLength > 0 ? maxLength : ProjectileID.Sets.TrailCacheLength[Type];
            var trail = new Vector2[trailLength];
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            for (int i = 0; i < trailLength; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    trailLength = i;
                    continue;
                }
                trail[i] = Projectile.oldPos[i] + offset;
            }
            foreach (var f in AequusHelpers.Circular(3, Main.GlobalTimeWrappedHourly * 5f))
            {
                var renderTrail = new Vector2[trailLength];
                Array.Copy(trail, renderTrail, trailLength);

                for (int i = 0; i < trailLength; i++)
                {
                    renderTrail[i] += new Vector2(0f, (float)Math.Sin(f + i * 0.33f) * waveSize).RotatedBy(Projectile.oldRot[i]);
                }

                prim.Draw(renderTrail);
            }
        }
    }
}