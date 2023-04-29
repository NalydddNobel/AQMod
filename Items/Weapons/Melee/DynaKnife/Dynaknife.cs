using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.DynaKnife {
    [AutoloadGlowMask]
    public class Dynaknife : ModItem {
        public override void SetStaticDefaults() {
            AequusItem.HasWeaponCooldown.Add(Type);
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.SetWeaponValues(40, 0.1f, 46);
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
            Item.shootSpeed = 2f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<DynaknifeProj>();
            Item.scale = 1.1f;
        }

        public override bool AltFunctionUse(Player player) {
            return !player.Aequus().HasCooldown;
        }

        public override bool? UseItem(Player player) {
            if (player.altFunctionUse == 2) {
                int dir = player.direction;
                if (Main.myPlayer == player.whoAmI) {
                    dir = Math.Sign(Main.MouseWorld.X - player.Center.X);
                }
                player.velocity.X = 12f * dir;
                player.Aequus().SetCooldown(120);
            }
            return null;
        }
    }

    public class DynaknifeProj : SwordProjectileBase {
        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.extraUpdates = 3;
            swordHeight = 44;
            swordWidth = 32;
            gfxOutOffset = -2;
            amountAllowedToHit = 1;
        }

        public override bool? CanDamage() {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.NewProjectile(
                Projectile.GetSource_OnHit(target),
                Main.player[Projectile.owner].Center,
                Projectile.velocity,
                ModContent.ProjectileType<DynaknifeStabProj>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner,
                target.whoAmI
            );
            var player = Main.player[Projectile.owner];
            player.immuneTime = Math.Max(player.immuneTime, 12);
            player.immuneNoBlink = true;
            player.velocity.X = -player.direction * 8f;
            SoundEngine.PlaySound(AequusSounds.inflictBlood, Projectile.Center);
        }

        protected override void Initialize(Player player, AequusPlayer aequus) {
            swingDirection = -player.direction;
            rotationOffset = -MathHelper.PiOver4 * swingDirection;
        }

        public override void AI() {
            base.AI();

            if (freezeFrame > 0)
                return;

            if (!playedSound) {
                playedSound = true;
                SoundEngine.PlaySound(AequusSounds.dagger, Projectile.Center);
            }
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        }

        public override Vector2 GetOffsetVector(float progress) {
            return base.GetOffsetVector(progress).RotatedBy(swingDirection * 0.8f);
        }

        public override float SwingProgress(float progress) {
            return 1f - MathF.Pow(1f - progress, 3f);
        }

        public override float GetScale(float progress) {
            return base.GetScale(progress);
        }

        public override float GetVisualOuter(float progress, float swingProgress) {
            return 0f;
        }

        public override bool PreDraw(ref Color lightColor) {
            var glowColor = new Color(120, 20, 36, 66);
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            float animProgress = AnimProgress;
            float swishProgress = 0f;
            float intensity = 0f;
            if (animProgress > 0f) {
                swishProgress = 1f - MathF.Pow(1f - animProgress, 5f);
                intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
            }

            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out float rotOffset, out var origin, out var effects);
            origin.X = Math.Clamp(origin.X, 4f, frame.Width - 4f);
            if (Aequus.HQ) {
                DrawSwordAfterImages(texture, handPosition, frame, glowColor * 0.4f * Projectile.Opacity, rotOffset, origin, effects,
                    loopProgress: 0.15f, interpolationValue: -0.05f);
            }
            DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotOffset, origin, effects);

            if (intensity > 0f) {
                var swish = AequusTextures.Swish.Value;
                var swishOrigin = swish.Size() / 2f;
                float r = BaseAngleVector.ToRotation();
                r += (MathF.Sin(animProgress * MathHelper.PiOver2) - 0.5f - 0.05f) * -swingDirection * 5f;
                var swishLocation = Main.player[Projectile.owner].Center - new Vector2(0f, Main.player[Projectile.owner].gfxOffY) - Main.screenPosition;

                Main.EntitySpriteDraw(
                    swish,
                    swishLocation + r.ToRotationVector2() * 24f * Projectile.scale,
                    null, 
                    glowColor * MathF.Pow(intensity, 3f), 
                    r + MathHelper.PiOver2, 
                    swishOrigin, 
                    new Vector2(0.5f, 0.6f), 
                    effects,
                    0
                );
            }
            return false;
        }
    }

    public class DynaknifeStabProj : ModProjectile {
        public const int ExplodeDelay = 120;

        public Vector2 normalOffset;

        public int NPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override string Texture => AequusTextures.DynaknifeProj.Path;

        public override void SetDefaults() {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = ExplodeDelay;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            normalOffset = default;
        }

        public override void AI() {
            var npc = Main.npc[NPC];

            if (npc.active) {
                Projectile.localAI[0] *= 0.8f;
                Projectile.localAI[0] -= 0.1f;
                if (normalOffset == default) {
                    Projectile.localAI[0] = 10f;
                    normalOffset = Projectile.DirectionFrom(npc.Center).SafeNormalize(Vector2.UnitY).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                }
                Projectile.Center = npc.Center;
                var dustPosition = Projectile.Center + normalOffset * Main.npc[NPC].Size / 2f;
                float dustVelocity = 16f * MathF.Pow(Projectile.timeLeft / (float)ExplodeDelay, 2f);
                var d = Dust.NewDustPerfect(dustPosition, DustID.Torch, normalOffset.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(dustVelocity));
                d.noGravity = true;
                d.fadeIn = d.scale + 0.2f;
                if (Projectile.timeLeft <= 40) {
                    float distance = Main.rand.NextFloat(Projectile.timeLeft * 2f);
                    var v = Main.rand.NextVector2Unit();
                    d = Dust.NewDustPerfect(dustPosition + v * distance, DustID.Torch, v * -distance * Main.rand.NextFloat(0.1f));
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                }
            }
            else {
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 5);
            }

            if (Projectile.timeLeft == 5) {
                Projectile.friendly = true;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                var dustPosition = Projectile.Center;
                for (int i = 0; i < Projectile.width; i++) {
                    float distance = Main.rand.NextFloat(Projectile.width / 2f);
                    var v = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(dustPosition + v * distance, DustID.Smoke, v * distance * Main.rand.NextFloat(0.1f), Scale: Main.rand.NextFloat(2f));
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                }
                for (int i = 0; i < Projectile.width; i++) {
                    float distance = Main.rand.NextFloat(Projectile.width / 2f);
                    var v = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(dustPosition + v * distance, DustID.Torch, v * distance * Main.rand.NextFloat(0.2f), Scale: Main.rand.NextFloat(2f));
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                }
                ScreenShake.SetShake(10f, where: Projectile.Center);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            if (Projectile.localAI[0] > 0f) {
                overPlayers.Add(index);
                Projectile.hide = true;
                return;
            }

            Projectile.hide = false;
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out _, out var frame, out var origin, out _);

            var npc = Main.npc[NPC];
            var drawPosition = npc.Center + new Vector2(0f, npc.gfxOffY) + normalOffset * Main.npc[NPC].Size / 2f;
            var effects = SpriteEffects.None;
            float rotation = normalOffset.ToRotation() + MathHelper.PiOver4 * 5f;
            float opacity = 1f;
            if (Projectile.localAI[0] > 0f) {
                Main.EntitySpriteDraw(
                    AequusTextures.Flare,
                    drawPosition - Main.screenPosition,
                    null,
                    Color.Red with { A = 100 } * Projectile.scale,
                    MathHelper.PiOver2,
                    AequusTextures.Flare.Size() / 2f,
                    new Vector2(0.5f, Projectile.localAI[0] * 0.5f) * Projectile.scale,
                    SpriteEffects.None,
                    0
                );
                opacity = 1f - Projectile.localAI[0] / 10f;
                drawPosition += new Vector2(Main.rand.NextFloat(-Projectile.localAI[0], Projectile.localAI[0]), Main.rand.NextFloat(-Projectile.localAI[0], Projectile.localAI[0]));
                drawPosition += normalOffset * Projectile.localAI[0] * 3f;
            }
            if (Projectile.timeLeft < 15) {
                float bloomOpacity = opacity * (1f - (Projectile.timeLeft - 5f) / 10f);
                Main.EntitySpriteDraw(
                    AequusTextures.Bloom0,
                    drawPosition - Main.screenPosition,
                    null,
                    Color.Red with { A = 0 } * bloomOpacity,
                    rotation,
                    AequusTextures.Bloom0.Size() / 2f,
                    Projectile.scale * bloomOpacity * 0.4f,
                    effects,
                    0
                );
            }

            Main.EntitySpriteDraw(
                texture,
                drawPosition - Main.screenPosition,
                frame,
                lightColor * opacity,
                rotation,
                origin,
                Projectile.scale * opacity,
                effects,
                0
            );
            return false;
        }
    }
}