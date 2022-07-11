using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Items.Weapons.Melee;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords
{
    public class UltimateSwordProj : SwordProjectileBase
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 70;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.extraUpdates = 10;
            Projectile.localNPCHitCooldown *= 10;
            hitboxOutwards = 150;
            rotationOffset = -MathHelper.PiOver4 * 3f;
        }

        protected override void Initialize(Player player, AequusPlayer aequus)
        {
            base.Initialize(player, aequus);
            Projectile.scale += Main.rand.NextFloat(-0.4f, 0.5f);
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
            if (aequus.itemCombo > 0)
            {
                swingDirection *= -1;
            }
        }

        public override void AI()
        {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - (MathHelper.PiOver2 * 1.5f)) * -swingDirection);
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (progress > 0.85f)
            {
                Projectile.Opacity = 1f - (progress - 0.85f) / 0.15f;
            }

            if (progress > 0.4f && progress < 0.6f)
            {
                var car = new Color[] { new Color(0, 255, 0), new Color(100, 255, 255), new Color(200, 0, 255) };
                int amt = !Aequus.HQ ? 1 : Main.rand.Next(2) + 1;
                for (int i = 0; i < amt; i++)
                {
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f), newColor: AequusHelpers.LerpBetween(car, Main.rand.NextFloat(3f)).UseA(0));
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale / 2f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
            }

            Projectile.oldPos[0] = AngleVector * 60f * Projectile.scale;
            Projectile.oldRot[0] = Projectile.oldPos[0].ToRotation() + MathHelper.PiOver4;

            // Manually updating oldPos and oldRot 
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
        }

        public override float SwingProgress(float progress)
        {
            return GenericSwing2(progress);
        }
        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f)
            {
                return scale + 2f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress)
        {
            //if (progress > 0.8f)
            //{
            //    float p = 1f - (1f - progress) / 0.2f;
            //    Projectile.alpha = (int)(p * 255);
            //    return -40f * p;
            //}
            //if (progress < 0.2f)
            //{
            //    return -10f * (1f - progress / 0.2f);
            //}
            return 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                AequusEffects.Shake.Set(4f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var car = new Color[] { new Color(0, 255, 0), new Color(100, 255, 255), new Color(200, 0, 255) };
            var armTrail = new TrailRenderer(TextureCache.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(25f) * Projectile.scale, (p) => Color.Lerp(new Color(120, 255, 120), new Color(255, 120, 255), AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f - p * 3f, 0.33f, 0.75f)).UseA(0) * 0.33f * Projectile.Opacity * (float)Math.Pow((1f - p), 2f) * Projectile.Opacity, drawOffset: Projectile.Size / 2f);
            var armTrailSmoke = new SwordSlashPrimRenderer(TextureCache.Trail[3].Value, TrailRenderer.DefaultPass, (p) => new Vector2(30f) * Projectile.scale, (p) => Color.Lerp(new Color(10, 255, 10), new Color(255, 10, 255), AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f - p * 3f, 0.33f, 0.75f)).UseA(0) * 0.33f * Projectile.Opacity * (float)Math.Pow((1f - p), 2f), drawOffset: Projectile.Size / 2f)
            {
                coord1 = 0f,
                coord2 = 1f
            };

            var greal = AequusHelpers.LerpBetween(car, Main.GlobalTimeWrappedHourly * 5f);
            var texture = TextureAssets.Projectile[Type].Value;
            var center = Main.player[Projectile.owner].Center;
            var handPosition = Main.GetPlayerArmPosition(Projectile) + AngleVector * visualOutwards;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var drawCoords = handPosition - Main.screenPosition;
            float size = texture.Size().Length();
            var effects = SpriteEffects.None;
            bool flip = Main.player[Projectile.owner].direction == 1 ? combo > 0 : combo == 0;
            if (flip)
            {
                Main.instance.LoadItem(ModContent.ItemType<UltimateSword>());
                texture = TextureAssets.Item[ModContent.ItemType<UltimateSword>()].Value;
            }

            var glowmask = ModContent.Request<Texture2D>(flip ? AequusHelpers.GetPath<UltimateSword>() + "_Glow" : Texture + "_Glow");
            var origin = new Vector2(0f, texture.Height);

            armTrail.drawOffset = handPosition;
            armTrailSmoke.drawOffset = handPosition;

            var bloom = TextureCache.Bloom[1].Value;
            Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.3f, Projectile.scale), effects, 0);
            if (Aequus.HQ)
            {
                Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.4f, Projectile.scale * 1.1f), effects, 0);
                Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, greal * 0.4f, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.6f, Projectile.scale * 1.25f), effects, 0);
                Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, greal * 0.2f, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale, Projectile.scale * 1.5f), effects, 0);
            }

            Vector2[] circular = AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f * Projectile.scale, null, AequusHelpers.LerpBetween(car, Main.GlobalTimeWrappedHourly * 5f + i * 0.25f).UseA(0) * 0.33f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            Main.spriteBatch.End();
            Begin.GeneralEntities.BeginShader(Main.spriteBatch);

            armTrail.Draw(Projectile.oldPos);
            armTrail.Draw(Projectile.oldPos);
            if (Aequus.HQ)
            {
                armTrailSmoke.Draw(Projectile.oldPos);
                armTrailSmoke.Draw(Projectile.oldPos);
            }

            Main.spriteBatch.End();
            Begin.GeneralEntities.Begin(Main.spriteBatch);

            Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowmask.Value, handPosition - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (AnimProgress > 0.45f && AnimProgress < 0.65f)
            {
                float intensity = (float)Math.Sin((AnimProgress - 0.25f) * 2f * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity, Projectile.rotation, origin, Projectile.scale, effects, 0);

                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var shineOrigin = shine.Size() / 2f;
                var shineColor = greal.UseA(0) * 0.8f * intensity * intensity * Projectile.Opacity;
                var shineLocation = handPosition - Main.screenPosition + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((size - 8f) * Projectile.scale);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, 0f, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * intensity, effects, 0);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 2f) * intensity, effects, 0);
            }

            return false;
        }
    }
}