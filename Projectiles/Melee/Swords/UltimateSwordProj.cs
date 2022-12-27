using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
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
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.extraUpdates = 10;
            Projectile.localNPCHitCooldown *= 10;
            hitboxOutwards = 70;
            rotationOffset = -MathHelper.PiOver4 * 3f;
            Projectile.noEnchantmentVisuals = true;
            amountAllowedToHit = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        protected override void Initialize(Player player, AequusPlayer aequus)
        {
            base.Initialize(player, aequus);
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
            if (aequus.itemCombo > 0)
            {
                swingDirection *= -1;
            }
        }

        public override void AI()
        {
            forced50 = true;
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(HeavySwing.WithPitchOffset(0.4f), Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - (MathHelper.PiOver2 * 1.5f)) * -swingDirection * 0.95f);
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (progress > 0.85f)
            {
                Projectile.Opacity = 1f - (progress - 0.85f) / 0.15f;
            }

            if (progress > 0.33f && progress < 0.55f)
            {
                if (Projectile.numUpdates <= 2)
                {
                    var car = new Color[] { new Color(0, 255, 0), new Color(100, 255, 255), new Color(200, 0, 255) };
                    int amt = 1;
                    for (int i = 0; i < amt; i++)
                    {
                        var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 12f);
                        var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: AequusHelpers.LerpBetween(car, (Main.GlobalTimeWrappedHourly * 0.5f + Main.rand.NextFloat(0.5f)) % 3f).UseA(128));
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        d.scale *= Projectile.scale * 0.6f;
                        d.fadeIn = d.scale + 0.1f;
                        d.noGravity = true;
                        if (Projectile.numUpdates == -1)
                        {
                            AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                        }
                    }
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
                return scale + 0.5f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress)
        {
            return 0f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            AequusBuff.ApplyBuff<AethersWrath>(target, 360, out bool canPlaySound);
            if (canPlaySound)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketSystem.SyncSound(SoundPacket.InflictAetherFire, target.Center);
                }
                SoundEngine.PlaySound(AethersWrath.InflictDebuffSound, target.Center);
            }
            if (canPlaySound || target.HasBuff<AethersWrath>())
            {
                for (int i = 0; i < 12; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(target.Center + v * new Vector2(Main.rand.NextFloat(target.width / 2f + 16f), Main.rand.NextFloat(target.height / 2f + 16f)), DustID.AncientLight, v * 8f);
                    d.noGravity = true;
                    d.noLightEmittence = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var car = new Color[] { new Color(0, 255, 0), new Color(100, 255, 255), new Color(200, 0, 255) };

            var greal = AequusHelpers.LerpBetween(car, Main.GlobalTimeWrappedHourly * 0.5f);
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

            var glowmask = ModContent.Request<Texture2D>(flip ? AequusHelpers.GetPath<UltimateSword>() + "_Glow" : Texture + "_Glow", AssetRequestMode.ImmediateLoad);
            var origin = new Vector2(0f, texture.Height);

            var bloom = TextureCache.Bloom[1].Value;
            Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.3f, Projectile.scale), effects, 0);
            if (Aequus.HQ)
            {
                Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, Color.White * 0.5f * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.4f, Projectile.scale * 1.1f), effects, 0);
                Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, greal * 0.2f * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.6f, Projectile.scale * 1.25f), effects, 0);
                Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, greal * 0.1f * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale, Projectile.scale * 1.5f), effects, 0);
            }

            var circular = AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f * Projectile.scale, null, AequusHelpers.LerpBetween(car, Main.GlobalTimeWrappedHourly * 5f + i * 0.25f).UseA(0) * 0.33f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowmask.Value, handPosition - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (AnimProgress > 0.2f && AnimProgress < 0.8f)
            {
                float swishProgress = (AnimProgress - 0.2f) / 0.6f;
                float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity * 0.5f, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = Swish2Texture.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = greal.UseA(58) * 0.5f * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation() + (swishProgress * 2f - 1f) * -swingDirection * 0.5f;
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 30f + 20f * swishProgress) * scale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, new Vector2(2f, 2f), effects, 0);
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 60f + 20f * swishProgress) * scale, null, swishColor * 0.4f, r + MathHelper.PiOver2, swishOrigin, new Vector2(2.5f, 4f), effects, 0);
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