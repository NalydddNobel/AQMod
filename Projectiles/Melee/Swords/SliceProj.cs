using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords
{
    public class SliceProj : SwordProjectileBase
    {
        public int swingTime;
        public int swingTimeMax;

        public override float AnimProgress => 1f - swingTime / (float)swingTimeMax;

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
            hitboxOutwards = 45;
            visualOutwards = 8;
            rotationOffset = -MathHelper.PiOver4 * 3f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(222);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn2, 1000);
        }

        protected override void Initialize(Player player, AequusPlayer aequus)
        {
            base.Initialize(player, aequus);
            if (aequus.itemCombo > 0)
            {
                swingDirection *= -1;
            }
        }

        public override void AI()
        {
            if (swingTimeMax == 0)
            {
                swingTimeMax = (int)(Main.player[Projectile.owner].itemAnimationMax * Math.Clamp(1.5f - Main.player[Projectile.owner].Aequus().itemUsage / 600f, 0.66f, 1.5f));
                swingTime = swingTimeMax;
                scale += Main.rand.NextFloat(0.25f);
                int delay = 1;
                Main.player[Projectile.owner].itemTime = swingTimeMax + delay;
                Main.player[Projectile.owner].itemTimeMax = swingTimeMax + delay;
                Main.player[Projectile.owner].itemAnimation = swingTimeMax + delay;
                Main.player[Projectile.owner].itemAnimationMax = swingTimeMax + delay;
            }
            base.AI();

            float progress = AnimProgress;
            if (progress > 0.33f && progress < 0.55f)
            {
                for (int i = 0; i < 2; i++)
                {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 12f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: new Color(80, 155, 255, 128), Scale: 2f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale * 0.6f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                    if (i == 0)
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                }
            }

            if (swingTime <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? swingTimeMax : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(Aequus.GetSounds("Item/swordSwoosh", 7, 1f, 0.5f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)), Projectile.Center);
            }
            swingTime--;
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            var player = Main.player[Projectile.owner];
            if (progress == 0.5f && Main.myPlayer == Projectile.owner && player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                    AngleVector * Projectile.velocity.Length() * 15f,
                    ModContent.ProjectileType<Sliceflake>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            if (progress < 0.5f)
                return base.GetOffsetVector(progress);
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection * (0.7f + 0.2f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)));
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
                return scale + 0.4f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress)
        {
            if (progress > 0.8f)
            {
                float p = 1f - (1f - progress) / 0.2f;
                Projectile.alpha = (int)(p * 255);
                return -10f * p;
            }
            if (progress < 0.35f)
            {
                float p = 1f - (progress) / 0.35f;
                Projectile.alpha = (int)(p * 255);
                return -10f * p;
            }
            return 0f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var glowColor = new Color(80, 155, 255, 120);
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
                Main.instance.LoadItem(ModContent.ItemType<Slice>());
                texture = TextureAssets.Item[ModContent.ItemType<Slice>()].Value;
            }

            var origin = new Vector2(0f, texture.Height);

            var circular = AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f * Projectile.scale, null, glowColor * 0.33f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (AnimProgress > 0.2f && AnimProgress < 0.8f)
            {
                float swishProgress = (AnimProgress - 0.2f) / 0.6f;
                float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity * 0.5f, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = Swish2Texture.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = glowColor.UseA(58) * 0.5f * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation() + (swishProgress * 2f - 1f) * -swingDirection * (0.4f + 0.2f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f));
                float scaling = Math.Clamp(2.5f - Main.player[Projectile.owner].itemAnimationMax / 8f, 1f, 10f);
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 40f - 40f * (scaling - 1f) + 20f * swishProgress) * scale, null, swishColor * 1.25f, r + MathHelper.PiOver2, swishOrigin, 1.5f * scaling, effects, 0);
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 50f - 40f * (scaling - 1f) + 20f * swishProgress) * scale, null, swishColor * 0.7f, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.2f, 2f) * scaling, effects, 0);
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Projectile.scale = reader.ReadSingle();
        }
    }
}