using Aequus.Common.Configuration;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public sealed class SliceProj : DopeSwordBase
    {
        public override float VisualHoldout => 8f;
        public override float HitboxHoldout => 45f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.extraUpdates = 8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

        public override bool? CanDamage()
        {
            return SwingProgress > 0.4f && damageTime < 4;
        }

        public override void Initalize(Player player, AequusPlayer aequusPlayer)
        {
            base.Initalize(player, aequusPlayer);
            int direction = -Projectile.direction;
            if (combo > 0)
            {
                direction = -direction;
            }
            angleVector = BaseAngleVector.RotatedBy(MathHelper.PiOver2 * -direction);
        }

        protected override void UpdateSwing(Player player, AequusPlayer aequus)
        {
            if (damageTime == 1)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), player.Center, Projectile.velocity * 10f, ProjectileID.NorthPoleSnowflake, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            int direction = -Projectile.direction;
            if (combo > 0)
            {
                direction = -direction;
            }
            angleVector = AngleVector.RotatedBy(MathHelper.Pi * swing * swingMultiplier * direction);
        }

        protected override void OnReachMaxProgress()
        {
            if (swingMultiplier < 0.05f)
            {
                base.OnReachMaxProgress();
                Cooldowns().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            else
            {
                swingMultiplier *= 0.95f * Main.player[Projectile.owner].meleeSpeed;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var origin = new Vector2(0f, texture.Height);
            var handPosition = Main.player[Projectile.owner].Center + AngleVector * VisualHoldout;
            var drawColor = Projectile.GetAlpha(lightColor);
            var drawCoords = handPosition - Main.screenPosition;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            float size = texture.Size().Length();
            var fowardOffset = 0f;
            var effects = SpriteEffects.None;
            bool flip = Main.player[Projectile.owner].direction == 1 ? combo > 0 : combo == 0;
            if (flip)
            {
                Main.instance.LoadItem(ModContent.ItemType<Slice>());
                texture = TextureAssets.Item[ModContent.ItemType<Slice>()].Value;
            }
            if (ClientConfiguration.Instance.effectQuality >= 1f)
            {
                for (int i = 0; i < trailLength; i++)
                {
                    float progress = 1f - 1f / trailLength * i;
                    Main.EntitySpriteDraw(texture, Projectile.oldPos[i] - Main.screenPosition + (Projectile.oldRot[i] - MathHelper.PiOver4).ToRotationVector2() * (size * (1f - progress * 0.33f) + fowardOffset), null, new Color(40, 120, 200, 0) * progress, Projectile.oldRot[i], origin, Projectile.scale * progress * 0.33f, effects, 0);
                }
            }
            trailLength = (int)(trailLength * 0.85f);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - 1f / trailLength * i;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] - Main.screenPosition + (Projectile.oldRot[i] - MathHelper.PiOver4).ToRotationVector2() * (size * (1f - progress) + fowardOffset), null, new Color(10, 60, 100, 0) * progress, Projectile.oldRot[i], origin, Projectile.scale * progress, effects, 0);
            }
            foreach (var v in AequusHelpers.CircularVector(4, Main.GlobalTimeWrappedHourly))
            {
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f, null, new Color(10, 60, 100, 0), Projectile.rotation, origin, Projectile.scale, effects, 0);
            }
            Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (SwingProgress > 0.25f && SwingProgress < 0.75f)
            {
                float intensity = (float)Math.Sin((SwingProgress - 0.25f) * 2f * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity, Projectile.rotation, origin, Projectile.scale, effects, 0);

                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var shineOrigin = shine.Size() / 2f;
                var shineColor = new Color(40, 120, 200, 0) * intensity * intensity;
                var shineLocation = handPosition - Main.screenPosition + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (size - 4f);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, 0f, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * intensity, effects, 0);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 2f) * intensity, effects, 0);
            }

            return false;
        }
    }
}