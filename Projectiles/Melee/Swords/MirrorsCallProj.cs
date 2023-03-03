using Aequus.Items.Weapons.Melee;
using Aequus.Particles.Dusts;
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
    public class MirrorsCallProj : SwordProjectileBase
    {
        public override string Texture => Helper.GetPath<MirrorsCall>();

        public int swingTimePrev;
        public int swingTime;
        public int swingTimeMax;

        public override float AnimProgress => 1f - (swingTime * ExtraUpdates + Projectile.numUpdates + 1) / (float)(swingTimeMax * ExtraUpdates);

        public int ExtraUpdates => Projectile.extraUpdates + 1;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.localNPCHitCooldown *= 2;
            Projectile.extraUpdates = 10;
            swordReach = 80;
            visualOutwards = 12;
            rotationOffset = -MathHelper.PiOver4 * 3f;
            amountAllowedToHit = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(222);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            if (freezeFrame <= 0)
            {
                Main.player[Projectile.owner].itemTime--;
                Main.player[Projectile.owner].itemAnimation--;
            }
            freezeFrame = 4;
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
                rotationOffset += Main.rand.NextFloat(-0.1f, 0.1f);
                swingTimeMax = (int)(Main.player[Projectile.owner].itemAnimationMax * Math.Clamp(1.5f - Main.player[Projectile.owner].Aequus().itemUsage / 200f, 0.8f, 1.25f));
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
            if (progress > 0.38f && progress < 0.54f && freezeFrame <= 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (!Main.rand.NextBool(ExtraUpdates))
                        continue;
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 12f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 100f * Projectile.scale), DustID.SilverFlame, velocity,
                        newColor: Helper.GetRainbowColor(Projectile, Main.GlobalTimeWrappedHourly).UseA(0) * 0.75f, Scale: 2f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale * 0.6f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                    if (i == 0)
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                }
            }

            if (freezeFrame > 0 || Projectile.numUpdates != -1)
                return;

            if (swingTime <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? swingTimeMax : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(Aequus.GetSounds("Item/swordSwoosh", 7, 1f, 0.5f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)), Projectile.Center);
            }
            swingTimePrev = swingTime;
            swingTime--;
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            var player = Main.player[Projectile.owner];
            if (progress == 0.5f && Main.myPlayer == Projectile.owner && player.altFunctionUse != 2)
            {
                for (int i = -1; i <= 1; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                        AngleVector.RotatedBy(i * 0.15f) * Projectile.velocity.Length() * 10f,
                        ModContent.ProjectileType<MirrorsCallBullet>(), (int)(Projectile.damage * 0.33f), Projectile.knockBack / 4f, Projectile.owner);
                }
            }
            if (progress > 0.8f)
            {
                float p = 1f - (1f - progress) / 0.2f;
                Projectile.alpha = (int)(p * 255);
            }
            if (progress < 0.35f)
            {
                float p = 1f - (progress) / 0.35f;
                Projectile.alpha = (int)(p * 255);
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 2f) - MathHelper.PiOver2 * 2f) * -swingDirection * (0.9f + 0.1f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)));
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
                return scale + 0.7f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress)
        {
            if (progress > 0.8f)
            {
                float p = 1f - (1f - progress) / 0.2f;
                return -14f * p;
            }
            if (progress < 0.35f)
            {
                float p = 1f - progress / 0.35f;
                return -14f * p;
            }
            return 0f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var glowColor = Helper.GetRainbowColor(Projectile, Main.GlobalTimeWrappedHourly).UseA(0) * 0.75f;
            var texture = TextureAssets.Projectile[Type].Value;
            var center = Main.player[Projectile.owner].Center;
            var handPosition = Main.GetPlayerArmPosition(Projectile) + AngleVector * visualOutwards;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var drawCoords = handPosition - Main.screenPosition;
            float size = texture.Size().Length();
            var effects = SpriteEffects.None;

            var origin = new Vector2(0f, texture.Height);
            float swordGlow = 0f;
            if (AnimProgress > 0.2f && AnimProgress < 0.7f)
            {
                swordGlow = (AnimProgress - 0.2f) / 0.5f;
            }
            float intensity = (float)Math.Sin((float)Math.Pow(swordGlow, 2f) * MathHelper.Pi);
            var circular = Helper.CircularVector(4, Projectile.rotation);
            for (int i = 0; i < circular.Length; i++)
            {
                var v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f * Projectile.scale, null, glowColor * intensity * 0.6f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }
            var glowTexture = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value;
            float trailAlpha = 1f;
            for (float f = lastAnimProgress; f > 0f && f < 1f && trailAlpha > 0f; f += -0.0025f)
            {
                InterpolateSword(f, out var offsetVector, out float _, out float scale, out float outer);
                Main.EntitySpriteDraw(glowTexture, handPosition - Main.screenPosition, null, glowColor * Projectile.Opacity * 0.25f * trailAlpha * intensity, (handPosition - (handPosition + offsetVector * swordReach)).ToRotation() + rotationOffset, origin, scale, effects, 0);
                trailAlpha -= 0.07f;
            }

            Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowTexture, handPosition - Main.screenPosition, null, glowColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (intensity > 0f)
            {
                glowColor = Color.Lerp(glowColor, Color.White.UseA(0), 0.33f) * 1.5f;
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity * 0.5f, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = SwishTexture.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = glowColor.UseA(58) * 0.4f * MathF.Pow(intensity, 5f) * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation();
                float r2 = r + -swingDirection * swordGlow;
                float scaling = 1f;
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r2.ToRotationVector2() * (size - 156f - 20f * (scaling - 1f) + 130f * swordGlow) * scale, null, swishColor * 1.25f, r2 + MathHelper.PiOver2, swishOrigin, 1.5f * scaling, effects, 0);
                Main.EntitySpriteDraw(swish, swishLocation + r2.ToRotationVector2() * (size - 130f - 20f * (scaling - 1f) + 110f * swordGlow) * scale, null, swishColor * 0.7f, r2 + MathHelper.PiOver2, swishOrigin, new Vector2(2f, 2f) * scaling, effects, 0);
                r += (swordGlow * 2f - 1f) * -swingDirection * 0.7f;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 50f - 20f * (scaling - 1f)) * scale, null, swishColor * 3f, r + MathHelper.PiOver2, swishOrigin, new Vector2(3f, 4.5f) * scaling, effects, 0);
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

    public class MirrorsCallBullet : ModProjectile
    {
        public float colorProgress;

        public override string Texture => "Aequus/Assets/Bullet";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 75;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White.UseA(0) * Projectile.Opacity;
        }

        public override void AI()
        {
            var target = Projectile.FindTargetWithinRange(400f);
            if (target != null)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * 10f, 0.05f);
            }
            if (Projectile.timeLeft <= 30)
            {
                Projectile.alpha += 8;
                Projectile.velocity *= 0.95f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.numUpdates == 0 && Main.rand.NextBool(Projectile.alpha / 8 + 1))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, -Projectile.velocity.X, -Projectile.velocity.Y, Projectile.alpha,
                    newColor: Helper.GetRainbowColor(Projectile, Main.GlobalTimeWrappedHourly).UseA(0) * 0.75f, Scale: 2f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.position + new Vector2(Main.rand.NextFloat(target.width), Main.rand.NextFloat(target.height)), new Vector2(Projectile.direction, 0f), ModContent.ProjectileType<MirrorsCallExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var bloom = Textures.Bloom[0].Value;
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            var circular = Helper.CircularVector(8, Projectile.rotation);
            Projectile.GetDrawInfo(out var texture, out var _, out var _, out var origin, out int _);
            var drawCoords = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 4f * Projectile.scale, null, Helper.GetRainbowColor(Projectile, Main.GlobalTimeWrappedHourly + i * 0.1f).UseA(0) * 0.5f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawCoords, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class MirrorsCallExplosion : ModProjectile
    {
        public float colorProgress;

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

        public static void ExplosionEffects(int player, Vector2 location, float colorProgress, float scale)
        {
            int amt = (int)(90 * scale);
            for (int i = 0; i < amt; i++)
            {
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(40 * scale);
                var d = Dust.NewDustPerfect(location + v, ModContent.DustType<MonoDust>(), v / 2.5f, 0, Helper.GetRainbowColor(player, colorProgress + Main.rand.NextFloat(-0.2f, 0.2f)).UseA(0) * Main.rand.NextFloat(0.6f, 1.1f) * scale, Main.rand.NextFloat(0.8f, 1.8f));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
            var position = target.position + new Vector2(Main.rand.NextFloat(target.width), Main.rand.NextFloat(target.height));
            for (int i = 0; i < 30; i++)
            {
                var d = Dust.NewDustPerfect(position, DustID.SilverFlame, newColor: Helper.GetRainbowColor(Projectile, Main.GlobalTimeWrappedHourly).UseA(0), Scale: Main.rand.NextFloat(1.5f, 2f));
                d.velocity = r * i / 4f * (Main.rand.NextBool() ? -1f : 1f);
                d.noGravity = true;
            }
            Projectile.NewProjectile(Projectile.GetSource_Death(), position, new Vector2(Projectile.direction, 0f), ModContent.ProjectileType<MirrorsCallExplosion>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
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