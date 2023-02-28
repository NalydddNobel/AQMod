using Aequus;
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
    public class NettlebaneProj : SwordProjectileBase
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
            swordReach = 80;
            swordSize = 30;
            rotationOffset = -MathHelper.PiOver4 * 3f;
            Projectile.noEnchantmentVisuals = true;
            amountAllowedToHit = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
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
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - (MathHelper.PiOver2 * 1.5f)) * -swingDirection * 1.1f);
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (progress > 0.85f)
            {
                Projectile.Opacity = 1f - (progress - 0.85f) / 0.15f;
            }

            if (progress > 0.33f && progress < 0.55f)
            {
                if (Projectile.numUpdates <= 0)
                {
                    var car = new Color[] { new Color(0, 255, 0), new Color(255, 255, 0) };
                    int amt = 1;
                    for (int i = 0; i < amt; i++)
                    {
                        var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 12f);
                        var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: Helper.LerpBetween(car, (Main.GlobalTimeWrappedHourly * 2.5f + Main.rand.NextFloat(0.5f)) % 3f).UseA(128));
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        d.scale *= Projectile.scale * 1f;
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
            return GenericSwing3(progress);
        }
        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f)
            {
                return scale + 0.5f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.6f * MathHelper.Pi), 2f);
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
            for (int i = 0; i < 7; i++)
            {
                var r = Main.rand.NextVector2Unit() * (1f - 0.1f * i);
                for (int j = 0; j < 15; j++)
                {
                    var d = Dust.NewDustPerfect(target.Center, DustID.RichMahogany, r * j, Scale: 1.5f - 0.08f * j);
                    d.noGravity = true;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.25f);
                }
            }
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, new Vector2(Projectile.direction * 0.1f, 0f), ModContent.ProjectileType<NettlebaneExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var car = new Color[] { new Color(0, 255, 0), new Color(255, 255, 0) };

            var greal = Helper.LerpBetween(car, Main.GlobalTimeWrappedHourly * 2.5f);
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
                Main.instance.LoadItem(ModContent.ItemType<Nettlebane>());
                texture = TextureAssets.Item[ModContent.ItemType<Nettlebane>()].Value;
            }

            var origin = new Vector2(0f, texture.Height);

            var circular = Helper.CircularVector(4, Projectile.rotation);
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f * Projectile.scale, null, Helper.LerpBetween(car, Main.GlobalTimeWrappedHourly * 2.5f + i * 0.25f).UseA(0) * 0.33f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, drawCoords, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            float progress = AnimProgress;
            if (progress > 0.33f && progress < 0.8f)
            {
                float swishProgress = 1f - MathF.Pow(1f - (progress - 0.33f) / 0.55f, 2f);
                float progress2 = 1f - (float)Math.Pow(1f - swishProgress, 2f);
                float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity * 0.5f, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = SwishTexture.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = greal.UseA(58) * 0.4f * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation();
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 100f + 140f * swishProgress) * scale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, new Vector2(2f, 3f) * (0.6f + 0.7f * swishProgress), effects, 0);
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

    public class NettlebaneExplosion : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;
        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.timeLeft = 6;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
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