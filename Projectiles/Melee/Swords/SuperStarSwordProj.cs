using Aequus;
using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Items.Weapons.Melee;
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
    public class SuperStarSwordProj : SwordProjectileBase
    {
        public static Color[] DustColors => new Color[] { new Color(10, 60, 255, 0), new Color(10, 255, 255, 0), new Color(100, 180, 255, 0), };
        public override bool SwingSwitchDir => AnimProgress > 0.1f && AnimProgress < 0.3f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.noEnchantmentVisuals = true;
            swordReach = 50;
            rotationOffset = -MathHelper.PiOver4 * 3f;
        }

        protected override void Initialize(Player player, AequusPlayer aequus)
        {
            base.Initialize(player, aequus);
            if (aequus.itemCombo > 0)
            {
                swingDirection *= -1;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            return (AnimProgress > 0.05f && AnimProgress < 0.5f) ? null : false;
        }

        public override void AI()
        {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 64 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(HeavySwing.WithPitchOffset(0.2f), Projectile.Center);
            }

            if (AnimProgress < 0.3f)
            {
                var car = DustColors;
                int amt = !Aequus.HQ ? 1 : Main.rand.Next(2) + 1;
                for (int i = 0; i < amt; i++)
                {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: Helper.LerpBetween(car, Main.rand.NextFloat(3f)).UseA(0));
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                    if (Projectile.numUpdates == -1)
                    {
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                    }
                }
            }
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (interpolatedSwingProgress >= 0.5f && Projectile.ai[0] < 1f)
            {
                if (Main.player[Projectile.owner].Aequus().MaxLife && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.player[Projectile.owner].Center + BaseAngleVector * 30f,
                        BaseAngleVector * Projectile.velocity.Length() * 9f,
                        ModContent.ProjectileType<SuperStarSwordSlash>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
                }
                Projectile.ai[0] = 1f;
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.25f) - MathHelper.PiOver2 * 1.25f) * -swingDirection);
        }

        public override float SwingProgress(float progress)
        {
            return Math.Max((float)Math.Sqrt(Math.Sqrt(Math.Sqrt(GenericSwing2(progress)))), MathHelper.Lerp(progress, 1f, progress));
        }

        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f)
            {
                return scale + 0.5f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress)
        {
            if (progress > 0.6f)
            {
                float p = 1f - (1f - progress) / 0.4f;
                Projectile.alpha = (int)(p * 255);
                return -8f * p;
            }
            return 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            AequusBuff.ApplyBuff<BlueFire>(target, 240, out bool canPlaySound);
            if (canPlaySound)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketSystem.SyncSound(SoundPacket.InflictBurning, target.Center);
                }
                SoundEngine.PlaySound(BlueFire.InflictDebuffSound, target.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
                Main.instance.LoadItem(ModContent.ItemType<SuperStarSword>());
                texture = TextureAssets.Item[ModContent.ItemType<SuperStarSword>()].Value;
            }
            var origin = new Vector2(0f, texture.Height);

            Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (AnimProgress < 0.4f)
            {
                float swishProgress = AnimProgress / 0.4f;
                float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity * 0.5f, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = SwishTexture.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = new Color(10, 30, 100, 10) * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation() + (swishProgress * 2f - 1f) * -swingDirection * 0.4f;
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 40f + 30f * swishProgress) * scale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, 1f, effects, 0);
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 55f + 30f * swishProgress) * scale, null, swishColor * 0.4f, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.2f, 1.75f), effects, 0);
            }
            return false;
        }
    }
}
