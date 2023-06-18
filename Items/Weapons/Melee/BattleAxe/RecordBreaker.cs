using Aequus.Buffs;
using Aequus.Items.Materials.Gems;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.BattleAxe {
    public class RecordBreaker : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAequusSword<RecordBreakerProj>(40);
            Item.useTime /= 4;
            Item.SetWeaponValues(24, 9.5f, 16);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 20;
            Item.height = 20;
            Item.axe = 12;
            Item.tileBoost = 2;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return true;
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override bool CanShoot(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<BattleAxe>()
                .AddIngredient<OmniGem>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class RecordBreakerProj : HeldSlashingSwordProjectile {
        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.extraUpdates = 5;
            swordHeight = 96;
            gfxOutOffset = -18;
            Projectile.localNPCHitCooldown *= Projectile.MaxUpdates;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void AI() {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1) {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f) {
                playedSound = true;
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
            }
            if (Projectile.numUpdates == -1 && AnimProgress > 0.3f && AnimProgress < 0.6f) {
                int amt = !Aequus.HQ ? 1 : Main.rand.Next(4) + 1;
                for (int i = 0; i < amt; i++) {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: GetRainbowColor((int)Main.GameUpdateCount) with { A = 0 });
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                    if (Projectile.numUpdates == -1) {
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                    }
                }
            }
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        }

        public override Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * MathHelper.Pi - MathHelper.PiOver2) * -swingDirection * 1.7f);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            freezeFrame = 4;
            // Prevents infinite freezing
            if (target.realLife == -1 && target.aiStyle != NPCAIStyleID.Worm && !target.HasBuff<RecordBreakerBuff>()) {
                target.AddBuff(ModContent.BuffType<RecordBreakerBuff>(), 30);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var center = Main.player[Projectile.owner].Center;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var glowColor = GetRainbowColor((int)Main.GameUpdateCount) with { A = 0 } * 0.66f;
            float animProgress = AnimProgress;
            float swishProgress = 0f;
            float intensity = 0f;
            if (animProgress > 0.05f && animProgress < 0.95f) {
                swishProgress = (animProgress - 0.05f) / 0.9f;
                intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
            }
            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out float rotationOffset, out var origin, out var effects);
            if (freezeFrame > 0) {
                handPosition += new Vector2(Main.rand.Next(-freezeFrame, freezeFrame), Main.rand.Next(-freezeFrame, freezeFrame)) * 6f;
            }
            float size = texture.Size().Length();
            if (Aequus.HQ) {
                if (intensity > 0f) {
                    DrawSwordAfterImages(AequusTextures.RecordBreakerProj_Trail, handPosition, frame, glowColor * 0.4f * Projectile.Opacity * intensity, rotationOffset, origin, effects,
                        loopProgress: 0.04f, interpolationValue: -0.004f);
                }

                float auraOffsetMagnitude = (2f + intensity * 4f) * Projectile.scale * baseSwordScale;
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2) {
                    DrawSword(texture, handPosition + i.ToRotationVector2() * auraOffsetMagnitude, frame, glowColor * 0.33f * Projectile.Opacity, rotationOffset, origin, effects);
                }
            }
            DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);

            if (intensity > 0f) {
                var shine = AequusTextures.Flare;
                var shineOrigin = shine.Size() / 2f;
                var shineColor = glowColor * 2f * intensity * Projectile.Opacity;
                var shineLocation = handPosition - Main.screenPosition
                    + (AngleVector * (size - 42f) + AngleVector.RotatedBy(-swingDirection * MathHelper.PiOver2) * 24f) * Projectile.scale;
                float shineRotation = AngleVector.ToRotation();
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, shineRotation, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * intensity, effects, 0);
                Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, shineRotation + MathHelper.PiOver2, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 2f) * intensity, effects, 0);
            }
            return false;
        }

        private Color GetRainbowColor(int tick) {
            return Color.Red.HueAdd(tick / 30f).SaturationMultiply(0.5f) * 1.25f;
        }
    }

    public class RecordBreakerBuff : ModBuff {
        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.AddStandardMovementDebuffImmunities(Type);
            AequusBuff.PlayerStatusBuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().frozen = true;
        }
    }
}