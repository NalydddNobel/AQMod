using Aequus.Content;
using Aequus.Items.Materials.GaleStreams;
using Aequus.Projectiles.Base;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee {
    public class Slice : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAequusSword<SliceProj>(30);
            Item.SetWeaponValues(60, 2.5f, 6);
            Item.width = 20;
            Item.height = 20;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.ValueGaleStreams;
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return null;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(120);
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override bool AltFunctionUse(Player player) {
            return false;
        }

        public override void AddRecipes() {
            FrozenTear.UpgradeItemRecipe(this, ModContent.ItemType<CrystalDagger>());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return true;
        }
    }
}

namespace Aequus.Projectiles.Melee.Swords {
    public class SliceProj : HeldSlashingSwordProjectile {
        public override string Texture => AequusTextures.Slice.Path;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.extraUpdates = 6;
            swordHeight = 105;
            swordWidth = 20;
            gfxOutOffset = -12;
            hitsLeft = 3;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(222);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
            SliceOnHitEffect.SpawnOnNPC(Projectile, target);
            target.AddBuff(BuffID.Frostburn2, 1000);
            freezeFrame = Math.Max(8 - TimesSwinged / 2, 0);
        }

        protected override void InitializeSword(Player player, AequusPlayer aequus) {
            swingTimeMax = Math.Max(swingTimeMax - Math.Clamp(TimesSwinged, 0, 10), 10);
        }

        public override void AI() {
            base.AI();

            float progress = AnimProgress;
            if (Projectile.numUpdates == -1 && progress > 0.33f && progress < 0.55f) {
                for (int i = 0; i < 3; i++) {
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

            if (freezeFrame > 0)
                return;

            if (!playedSound && AnimProgress > 0.4f) {
                playedSound = true;
                SoundEngine.PlaySound(AequusSounds.swordSwoosh with { Volume = 0.8f, }, Projectile.Center);
            }
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
            if (Projectile.numUpdates != -1) {
                return;
            }

            var player = Main.player[Projectile.owner];
            if (progress == 0.5f && Main.myPlayer == Projectile.owner && player.altFunctionUse != 2) {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_HeldItem(), Projectile.Center,
                    AngleVector * Projectile.velocity.Length() * 15f,
                    ModContent.ProjectileType<SliceBulletProj>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
            }
        }

        public override Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.75f) - MathHelper.PiOver2 * 1.75f) * -swingDirection * (0.9f + 0.1f * Math.Min(TimesSwinged / 5f, 1f)));
        }

        public override float SwingProgress(float progress) {
            return SwingProgressSplit(progress);
        }
        public override float GetScale(float progress) {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f) {
                return scale + 0.25f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress) {
            return 0f;
        }

        public override bool PreDraw(ref Color lightColor) {
            var glowColor = new Color(80, 155, 255, 0);
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            float animProgress = AnimProgress;
            float swishProgress = 0f;
            float intensity = 0f;
            if (animProgress > 0.3f && animProgress < 0.65f) {
                swishProgress = (animProgress - 0.3f) / 0.35f;
                intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
            }

            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out float rotationOffset, out var origin, out var effects);
            if (Aequus.HQ) {
                DrawSwordAfterImages(texture, handPosition, frame, glowColor * 0.4f * Projectile.Opacity, rotationOffset, origin, effects,
                    loopProgress: 0.07f, interpolationValue: -0.01f);

                float auraOffsetMagnitude = (2f + intensity * 4f) * Projectile.scale * baseSwordScale;
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2) {
                    DrawSword(texture, handPosition + i.ToRotationVector2() * auraOffsetMagnitude, frame, glowColor * 0.33f * Projectile.Opacity, rotationOffset, origin, effects);
                }
            }
            DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);

            if (intensity > 0f) {
                var swish = AequusTextures.Swish.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = glowColor with { A = 58 } * 0.3f * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation() + (swishProgress * 2f - 1f) * -swingDirection * (0.1f + 0.1f * Math.Min(TimesSwinged / 5f, 1f));
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;

                Main.EntitySpriteDraw(
                    swish, 
                    swishLocation + r.ToRotationVector2() * (60f - 40f + 30f * swishProgress) * Projectile.scale, 
                    null, swishColor * 1.25f, r + MathHelper.PiOver2, swishOrigin, 1.5f, effects, 0);
                Main.EntitySpriteDraw(
                    swish, 
                    swishLocation + r.ToRotationVector2() * (60f - 40f + 30f * swishProgress) * Projectile.scale, 
                    null, swishColor * 0.7f, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.8f, 2.5f), effects, 0);
            }

            if (animProgress < 0.6f) {
                float flareIntensity = animProgress / 0.6f;
                var flareColor = glowColor with { A = 0 } * flareIntensity * swishProgress;
                DrawSwordTipFlare(handPosition, swordHeight - 16f, new Vector2(0.9f, 2f) * Helper.Wave(Main.GlobalTimeWrappedHourly * 40f, 0.8f, 1f) * flareIntensity, flareColor, 0.7f * flareIntensity, flareColor.HueAdd(0.07f) with { A = 0 });
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer) {
            base.SendExtraAI(writer);
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            base.ReceiveExtraAI(reader);
            Projectile.scale = reader.ReadSingle();
        }
    }

    public class SliceBulletProj : ModProjectile {
        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.scale = 1.1f;
            Projectile.tileCollide = false;
        }

        public override void AI() {
            if ((int)Projectile.ai[0] == 0) {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.ai[0]++;
            }

            if (Projectile.alpha > 40) {
                if (Projectile.extraUpdates > 0) {
                    Projectile.extraUpdates = 0;
                }
                if (Projectile.scale > 1f) {
                    Projectile.scale -= 0.02f;
                    if (Projectile.scale < 1f) {
                        Projectile.scale = 1f;
                    }
                }
            }
            Projectile.velocity *= 0.99f;
            Projectile.rotation += Projectile.velocity.Length() * 0.03f * Projectile.direction;
            int size = 90;
            bool collding = Collision.SolidCollision(Projectile.position + new Vector2(size / 2f, size / 2f), Projectile.width - size, Projectile.height - size);
            if (collding) {
                Projectile.alpha += 8;
                Projectile.velocity *= 0.8f;
            }
            Projectile.alpha += 8;
            if (Projectile.alpha >= 255) {
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 10;
            height = 10;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -oldVelocity.X, 0.75f);
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -oldVelocity.Y, 0.75f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.damage = (int)(Projectile.damage * 0.5f);
            target.AddBuff(BuffID.Frostburn2, 480);
            SliceOnHitEffect.SpawnOnNPC(Projectile, target);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) {
            target.AddBuff(BuffID.Frostburn2, 480);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPosition = Projectile.Center;
            var drawOffset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            lightColor = Projectile.GetAlpha(lightColor);
            var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            frame.Height -= 2;
            var origin = frame.Size() / 2f;
            float opacity = Projectile.Opacity;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength; i++) {
                float progress = 1f - 1f / trailLength * i;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + drawOffset, frame, new Color(20, 80, 175, 10) * progress * opacity, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.position + drawOffset, frame, Projectile.GetAlpha(lightColor) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.instance.LoadProjectile(ProjectileID.NightsEdge);
            var swish = TextureAssets.Projectile[ProjectileID.NightsEdge].Value;
            var swishFrame = swish.Frame(verticalFrames: 4);
            var swishColor = new Color(60, 120, 255, 0) * opacity;
            var swishOrigin = swishFrame.Size() / 2f;
            float swishScale = Projectile.scale * 1f;
            var swishPosition = Projectile.position + drawOffset;

            var flare = AequusTextures.Flare.Value;
            var flareOrigin = flare.Size() / 2f;
            float flareOffset = (swish.Width / 2f - 4f);
            var flareDirectionNormal = Vector2.Normalize(Projectile.velocity) * flareOffset;
            float flareDirectionDistance = 200f;
            for (int i = 0; i < 2; i++) {
                float swishRotation = Projectile.rotation + MathHelper.Pi * i;
                Main.EntitySpriteDraw(
                    swish,
                    swishPosition,
                    swishFrame,
                    swishColor,
                    swishRotation,
                    swishOrigin,
                    swishScale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(
                    swish,
                    swishPosition,
                    swishFrame.Frame(0, 3),
                    swishColor,
                    swishRotation,
                    swishOrigin,
                    swishScale, SpriteEffects.None, 0);

                for (int j = 0; j < 3; j++) {
                    var flarePosition = (swishRotation + 0.6f * (j - 1)).ToRotationVector2() * flareOffset;
                    float flareIntensity = Math.Max(flareDirectionDistance - Vector2.Distance(flareDirectionNormal, flarePosition), 0f) / flareDirectionDistance;
                    Main.EntitySpriteDraw(
                        flare,
                        swishPosition + flarePosition,
                        null,
                        swishColor * flareIntensity * 3f * 0.4f,
                        0f,
                        flareOrigin,
                        new Vector2(swishScale * 0.7f, swishScale * 2f) * flareIntensity, SpriteEffects.None, 0);

                    Main.EntitySpriteDraw(
                        flare,
                        swishPosition + flarePosition,
                        null,
                        swishColor * flareIntensity * 3f * 0.4f,
                        MathHelper.PiOver2,
                        flareOrigin,
                        new Vector2(swishScale * 0.8f, swishScale * 2.5f) * flareIntensity, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        public override void Kill(int timeLeft) {
            if (Projectile.alpha < 200) {
                for (int i = 0; i < 30; i++) {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, newColor: new Color(80, 155, 255, 128), Scale: 2f);
                    d.velocity *= 0.4f;
                    d.velocity += Projectile.velocity * 0.5f;
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale * 0.6f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
            }
        }
    }

    public class SliceOnHitEffect : ModProjectile {

        public override string Texture => AequusTextures.Flare.Path;

        private bool _playedSound;

        public static int SpawnOnNPC(Projectile projectile, NPC target) {
            var v = Main.rand.NextVector2Unit();
            var size = target.Size;
            size.X = Math.Max(size.X, 80f);
            size.Y = Math.Max(size.Y, 80f);
            return Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center - v * size, v * size / 8f,
                ModContent.ProjectileType<SliceOnHitEffect>(), projectile.damage, projectile.knockBack, projectile.owner);
        }

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 16;
        }

        public override void AI() {
            if (!_playedSound) {
                _playedSound = true;
                SoundEngine.PlaySound(AequusSounds.hit_Sword.Sound with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3, }, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            float intensity = MathF.Pow(MathF.Sin(Projectile.timeLeft / 16f * MathHelper.Pi), 2f);
            var color = new Color(20, 200, 255, 0) * intensity;
            Main.EntitySpriteDraw(
                texture,
                Projectile.position + offset - Main.screenPosition,
                frame,
                color,
                Projectile.velocity.ToRotation() + MathHelper.PiOver2,
                origin,
                new Vector2(0.8f, 2f), SpriteEffects.None, 0
            );
            Main.EntitySpriteDraw(
                texture,
                Projectile.position + offset - Main.screenPosition,
                frame,
                color,
                Projectile.velocity.ToRotation() + MathHelper.PiOver2,
                origin,
                new Vector2(1f, 3f), SpriteEffects.None, 0
            );
            return false;
        }
    }
}