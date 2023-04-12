using Aequus;
using Aequus.Buffs.Misc;
using Aequus.Content.WorldGeneration;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Heavy
{
    public class Nettlebane : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            HardmodeChestBoost.HardmodeJungleChestLoot.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<NettlebaneProj>(40);
            Item.SetWeaponValues(92, 2.5f);
            Item.width = 30;
            Item.height = 30;
            Item.scale = 1.1f;
            Item.rare = ItemDefaults.RarityPreMechs;
            Item.value = ItemDefaults.ValueEarlyHardmode;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.HasBuff<NettlebaneBuffTier2>() || player.HasBuff<NettlebaneBuffTier3>())
            {
                return;
            }

            player.AddBuff(ModContent.BuffType<NettlebaneBuffTier1>(), 2, quiet: true);
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}

namespace Aequus.Projectiles.Melee.Swords
{
    public class NettlebaneProj : SwordProjectileBase
    {
        public int tier;
        public bool upgradingEffect;
        public bool hitAnything;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 70;
            ProjectileID.Sets.TrailingMode[Type] = -1;
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.extraUpdates = 10;
            Projectile.localNPCHitCooldown *= 10;
            swordReach = 85;
            swordSize = 10;
            rotationOffset = -MathHelper.PiOver4 * 3f;
            Projectile.noEnchantmentVisuals = true;
            amountAllowedToHit = 3;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage() {
            if (tier == 0) {
                return (AnimProgress > 0.05f && AnimProgress < 0.5f) ? null : false;
            }
            return base.CanDamage();
        }

        protected override void Initialize(Player player, AequusPlayer aequus)
        {
            base.Initialize(player, aequus);
            tier = 0;
            if (player.HasBuff(ModContent.BuffType<NettlebaneBuffTier2>()))
            {
                tier = 1;
            }
            if (player.HasBuff(ModContent.BuffType<NettlebaneBuffTier3>()))
            {
                tier = 2;
            }
            swordReach += 24 * tier;
            swordSize += 2 * tier;
            Projectile.width += 45 * tier;
            Projectile.height += 45 * tier;
            player.itemTimeMax += 3 * tier;
            player.itemAnimationMax += 3 * tier;
            player.itemAnimation = player.itemAnimationMax;
            player.itemTime = player.itemTimeMax;
            Projectile.damage = (int)(Projectile.damage * (1f + 0.5f * tier));
            if (aequus.itemCombo > 0)
            {
                swingDirection *= -1;
            }
        }

        public override void AI()
        {
            forced50 = true;
            base.AI();
            Projectile.frame = tier;
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
            if (tier <= 1) {
                return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection);
            }
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection * 1.1f);
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (progress > 0.85f)
            {
                Projectile.Opacity = 1f - (progress - 0.85f) / 0.15f;
            }

            if (Projectile.numUpdates <= tier) {

                float min = 0.33f;
                float max = 0.55f;
                if (tier == 0) {
                    min = 0f;
                    max = 0.2f;
                }

                if (progress > min && progress < max) {
                    int amt = 1;
                    Color greal = new(0, 200, 0, 128);
                    float maxDistance = 55f * Projectile.scale;
                    maxDistance *= 1f + tier * 0.5f;
                    for (int i = 0; i < amt; i++) {
                        var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, maxDistance / 12f) + BaseAngleVector * Main.rand.NextFloat(5f);
                        var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, maxDistance), DustID.TintableDustLighted, velocity, newColor: greal);
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        d.scale *= Projectile.scale + Main.rand.NextFloat(maxDistance / 100f);
                        d.fadeIn = d.scale * 0.6f;
                        d.noGravity = true;
                        if (Projectile.numUpdates == -1) {
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
            if (tier == 0) {
                return Math.Max((float)Math.Sqrt(Math.Sqrt(Math.Sqrt(GenericSwing2(progress)))), MathHelper.Lerp(progress, 1f, progress));
            }
            return GenericSwing3(MathF.Pow(progress, 1f));
        }
        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress) * 0.77f;
            if (progress > 0.1f && progress < 0.9f)
            {
                return scale + 0.3f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.6f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress)
        {
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (hitAnything)
            {
                return;
            }
            freezeFrame = 4 * tier;
            hitAnything = true;
            var player = Main.player[Projectile.owner];
            if (player.HasBuff<NettlebaneBuffTier3>())
            {
                SoundEngine.PlaySound(AequusSounds.largeSlash with { Volume = 0.44f, PitchVariance = 0.1f, }, target.Center);
                player.AddBuff(ModContent.BuffType<NettlebaneBuffTier3>(), 300);
            }
            else if (player.HasBuff<NettlebaneBuffTier2>())
            {
                SoundEngine.PlaySound(AequusSounds.swordPowerReady.Sound with { Volume = 0.8f, Pitch = 0.1f, MaxInstances = 2 }, target.Center);
                player.AddBuff(ModContent.BuffType<NettlebaneBuffTier3>(), 300);
                upgradingEffect = true;
            }
            else
            {
                SoundEngine.PlaySound(AequusSounds.swordPowerReady.Sound with { Volume = 0.8f, MaxInstances = 2 }, target.Center);
                player.AddBuff(ModContent.BuffType<NettlebaneBuffTier2>(), 300);
                upgradingEffect = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color greal = new(60, 255, 60, 255);
            var texture = TextureAssets.Projectile[Type].Value;
            var center = Main.player[Projectile.owner].Center;
            var handPosition = Main.GetPlayerArmPosition(Projectile) + AngleVector * visualOutwards;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var drawCoords = handPosition - Main.screenPosition;
            if (freezeFrame > 0)
            {
                drawCoords += new Vector2(Main.rand.NextFloat(-freezeFrame, freezeFrame), Main.rand.NextFloat(-freezeFrame, freezeFrame));
            }
            var effects = SpriteEffects.None;

            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            frame.Width /= 2;
            frame.X = frame.Width * (swingDirection == -1 ? 0 : 1);
            var origin = new Vector2(0f, frame.Height);
            float size = frame.Size().Length() * (0.33f + tier * 0.15f);

            var circular = Helper.CircularVector(4, Projectile.rotation);
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(
                    texture,
                    drawCoords + v * 2f * Projectile.scale,
                    frame,
                    greal with { A = 0 } * 0.33f * Projectile.Opacity,
                    Projectile.rotation,
                    origin,
                    Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, drawCoords, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            float progress = AnimProgress;

            float swishRangeMin = 0.2f;
            float swishRangeMax = 0.8f;
            if (tier == 0) {
                swishRangeMin = 0f;
                swishRangeMax = 0.6f;
            }
            if (progress > swishRangeMin && progress < swishRangeMax)
            {
                float swishProgress = 1f - MathF.Pow(1f - (progress - swishRangeMin) / (swishRangeMax - swishRangeMin), 2f);
                float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);
                Main.EntitySpriteDraw(texture, drawCoords, frame, new Color(50, 255, 50, 0) * intensity, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = SwishTexture.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = greal.UseA(58) * 0.1f * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation() + (swishProgress - 0.5f) * 0.33f * -swingDirection;
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(
                    swish,
                    swishLocation + r.ToRotationVector2() * (size * 0.5f + 20f * swishProgress) * scale,
                    null,
                    swishColor,
                    r + MathHelper.PiOver2,
                    swishOrigin,
                    new Vector2(size / 40f, size / 30f), effects, 0);
            }

            if (upgradingEffect && freezeFrame == 0)
            {
                var flare = AequusTextures.ShinyFlashParticle.Value;
                var flareOrigin = flare.Size() / 2f;
                float r = AngleVector.ToRotation() + 0.1f * tier * swingDirection;
                float offset = (1.7f - 0.25f * tier) * progress;
                var flareLocation = Main.player[Projectile.owner].Center - Main.screenPosition + r.ToRotationVector2() * size * offset;
                var flareColor = new Color(90, 255, 90, 40) * Projectile.Opacity * 0.33f;
                float flareScale = Projectile.scale * Projectile.Opacity;
                Main.EntitySpriteDraw(
                    flare,
                    flareLocation,
                    null,
                    flareColor,
                    offset,
                    flareOrigin,
                    flareScale, effects, 0);
                Main.EntitySpriteDraw(
                    flare,
                    flareLocation,
                    null,
                    flareColor,
                    MathHelper.PiOver2 + offset,
                    flareOrigin,
                    flareScale, effects, 0);
                Main.EntitySpriteDraw(
                    flare,
                    flareLocation,
                    null,
                    flareColor,
                    MathHelper.PiOver2,
                    flareOrigin,
                    new Vector2(flareScale * 0.5f, flareScale * 2.2f), effects, 0);
            }
            //DrawDebug();
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

    //public class NettlebaneOnHitProj : ModProjectile
    //{
    //    public override string Texture => Aequus.BlankTexture;
    //    public override void SetDefaults()
    //    {
    //        Projectile.width = 160;
    //        Projectile.height = 160;
    //        Projectile.timeLeft = 6;
    //        Projectile.friendly = true;
    //        Projectile.tileCollide = false;
    //        Projectile.ignoreWater = true;
    //        Projectile.DamageType = DamageClass.Melee;
    //        Projectile.usesIDStaticNPCImmunity = true;
    //        Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
    //        Projectile.penetrate = -1;
    //    }

    //    public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
    //    {
    //    }

    //    public override void SendExtraAI(BinaryWriter writer)
    //    {
    //        base.SendExtraAI(writer);
    //        writer.Write(Projectile.scale);
    //    }

    //    public override void ReceiveExtraAI(BinaryReader reader)
    //    {
    //        base.ReceiveExtraAI(reader);
    //        Projectile.scale = reader.ReadSingle();
    //    }
    //}
}

namespace Aequus.Buffs.Misc
{
    public class NettlebaneBuffTier1 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 12;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }

    public class NettlebaneBuffTier2 : NettlebaneBuffTier1
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 18;
        }

        public override bool RightClick(int buffIndex)
        {
            return true;
        }
    }

    public class NettlebaneBuffTier3 : NettlebaneBuffTier2
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 24;
            player.ClearBuff(ModContent.BuffType<NettlebaneBuffTier2>());
        }

        public override bool RightClick(int buffIndex)
        {
            var player = Main.LocalPlayer;
            player.AddBuff(ModContent.BuffType<NettlebaneBuffTier2>(), player.buffTime[buffIndex]);
            return true;
        }
    }
}