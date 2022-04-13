using Aequus.Common.Configuration;
using Aequus.Effects.Prims;
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
        public SwordSlashPrimRenderer prim;
        public SwordSlashPrimRenderer primBlue;

        public override float Radius => 76.36f;
        public override float VisualHoldout => 8f;
        public override float HitboxHoldout => 45f;
        public override float AltFunctionSpeedup => 0.35f;
        public override float AltFunctionScale => 1.7f;

        public static bool FasterSwings(int itemUsage)
        {
            return itemUsage >= 60;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.extraUpdates = 16;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(222);
        }

        public override bool? CanDamage()
        {
            return SwingProgress > 0.4f && damageTime < 4;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 360);
        }

        protected override void AdjustSwingTime(Player player, AequusPlayer aequusPlayer)
        {
            if (!FasterSwings(aequusPlayer.itemUsage))
            {
                swingTimeMax = (int)(swingTimeMax * 1.5f);
            }
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
            if (damageTime == 1 && Main.myPlayer == player.whoAmI && player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), player.Center, Projectile.velocity * 10f, ModContent.ProjectileType<Sliceflake>(), (int)(Projectile.damage * 0.66f), Projectile.knockBack, Projectile.owner);
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
                Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            else
            {
                swingMultiplier *= 0.975f - (Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee) - 1f) / 8f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var origin = new Vector2(0f, texture.Height);
            var center = Main.player[Projectile.owner].Center;
            var handPosition = center + AngleVector * VisualHoldout;
            var drawColor = Projectile.GetAlpha(lightColor);
            var drawCoords = handPosition - Main.screenPosition;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            float size = texture.Size().Length();
            var effects = SpriteEffects.None;
            bool flip = Main.player[Projectile.owner].direction == 1 ? combo > 0 : combo == 0;
            if (flip)
            {
                Main.instance.LoadItem(ModContent.ItemType<Slice>());
                texture = TextureAssets.Item[ModContent.ItemType<Slice>()].Value;
            }
            float trailOutwards = texture.Size().Length() * Projectile.scale - 40f * Projectile.scale;
            bool reverseTrail = Projectile.direction == -1 ? combo > 0 : combo == 0;
            var oldPos = Array.ConvertAll(Projectile.oldPos, (v) => Vector2.Normalize(v) * trailOutwards);
            if (ClientConfiguration.Instance.HighQuality)
            {
                if (primBlue == null)
                {
                    primBlue = new SwordSlashPrimRenderer(TextureAssets.Extra[ExtrasID.EmpressBladeTrail].Value, LegacyPrimRenderer.DefaultPass, (p) => new Vector2(40f) * Projectile.scale, (p) => new Color(15, 75, 255, 75) * (1f - p) * (1f - p) * (1f - p));
                }
                if (reverseTrail)
                {
                    primBlue.coord1 = 0f;
                    primBlue.coord2 = 1f;
                }
                else
                {
                    primBlue.coord1 = 1f;
                    primBlue.coord2 = 0f;
                }
                primBlue.drawOffset = center;
                primBlue.Draw(oldPos);
            }
            if (prim == null)
            {
                prim = new SwordSlashPrimRenderer(TextureAssets.Extra[ExtrasID.EmpressBladeTrail].Value, LegacyPrimRenderer.DefaultPass, (p) => new Vector2(40f) * Projectile.scale, (p) => new Color(75, 180, 255, 0) * (1f - p));
            }
            if (reverseTrail)
            {
                prim.coord1 = 0f;
                prim.coord2 = 1f;
            }
            else
            {
                prim.coord1 = 1f;
                prim.coord2 = 0f;
            }
            prim.drawOffset = center;
            prim.Draw(oldPos);

            foreach (var v in AequusHelpers.CircularVector(4, Main.GlobalTimeWrappedHourly))
            {
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f, null, new Color(5, 25, 100, 0), Projectile.rotation, origin, Projectile.scale, effects, 0);
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
                var shineLocation = handPosition - Main.screenPosition + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * ((size - 8f) * Projectile.scale);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, 0f, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * intensity, effects, 0);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 2f) * intensity, effects, 0);
            }

            return false;
        }
    }
}