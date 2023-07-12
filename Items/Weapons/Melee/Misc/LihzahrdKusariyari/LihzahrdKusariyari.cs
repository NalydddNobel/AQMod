using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Net.Sounds;
using Aequus.Common.Projectiles.Base;
using Aequus.Items.Weapons.Melee.Swords.BattleAxe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Misc.LihzahrdKusariyari {
    public class LihzahrdKusariyari : ModItem {
        public override bool IsLoadingEnabled(Mod mod) {
            return Aequus.DevelopmentFeatures;
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.SetWeaponValues(180, 4f, 6);
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemDefaults.RarityTemple;
            Item.value = Item.buyPrice(gold: 10);
            Item.shootSpeed = 2f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<LihzahrdKusariyariProj>();
        }

        public override bool CanUseItem(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool AltFunctionUse(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            if (player.altFunctionUse == 2) {
                type = ModContent.ProjectileType<LihzahrdKusariyariAltProj>();
            }
        }
    }

    public class LihzahrdKusariyariProj : HeldSlashingSwordProjectile {
        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.extraUpdates = 10;
            swordHeight = 60;
            gfxOutOffset = -18;
        }

        public override void AI() {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1) {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 64 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f) {
                playedSound = true;
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.75f) - MathHelper.PiOver2 * 1.75f) * -swingDirection * (0.9f + 0.1f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)));
        }


        public override float SwingProgress(float progress) {
            return SwingProgressSplit(progress);
        }

        public override float GetScale(float progress) {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f) {
                return scale + 0.5f * Helper.Wave((progress - 0.1f) / 0.8f * MathHelper.Pi, 0f, 1f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress) {
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
            freezeFrame = 8;
            ScreenShake.SetShake(20f, 0.8f);
            if (Main.rand.NextBool(5)) {
                ModContent.GetInstance<BleedingDebuffSound>().Play(target.Center);
                target.AddBuff(ModContent.BuffType<BattleAxeBleeding>(), 240);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);

            float colorIntensity = drawColor.ToVector3().Length() * Projectile.Opacity * 0.2f;
            Color swishColor = new(colorIntensity, colorIntensity, colorIntensity, 0f);

            DrawSwordAfterImages(AequusTextures.LihzahrdKusariyariProj_Glow, handPosition, frame, Color.Yellow with { A = 0 } * 0.3f, rotationOffset, origin, effects, 0.1f, -0.01f);
            DrawSword(texture, handPosition, frame, drawColor, rotationOffset, origin, effects);
            return false;
        }
    }

    public class LihzahrdKusariyariAltProj : ModProjectile {
        public int StuckEnemy { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }

        public override void SetDefaults() {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.hide = true;
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            player.itemAnimation = Math.Max(player.itemAnimation, 2);
            player.itemTime = Math.Max(player.itemTime, 2);
            Projectile.position += player.velocity * 0.9f;
            if ((int)Projectile.ai[0] == 0) {
                Projectile.ai[1] = -1f;
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            }
            Projectile.rotation = Projectile.DirectionFrom(player).ToRotation();

            if (Projectile.localAI[0] > 0f) {
                Projectile.localAI[0] *= 0.9f;
                Projectile.localAI[0] -= 0.05f;
                if (Projectile.localAI[0] < 0f) {
                    Projectile.localAI[0] = 0f;
                }
            }

            int target = StuckEnemy;
            if (target > -1) {
                Projectile.Center = Main.npc[target].Center;
                Projectile.velocity *= 0.8f;
                return;
            }
            if ((int)Projectile.ai[0] == -1) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * 40f, 0.05f);
                if (Projectile.Distance(player) < 70f) {
                    Projectile.Kill();
                }
                return;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 10f) {
                Projectile.velocity *= 0.92f;
            }
            else {
                Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Torch
                );
            }
            if (Projectile.ai[0] > 20f) {
                Projectile.ai[0] = -1f;
            }
        }

        public override bool? CanDamage() {
            return StuckEnemy < 0f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 16;
            height = 16;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            StuckEnemy = target.whoAmI;
            Projectile.localAI[0] = 10f;
            Projectile.velocity = -Projectile.velocity;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.ai[0] = -1f;
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor) {
            var drawPosition = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + MathHelper.PiOver4;
            var effects = SpriteEffects.None;
            var dir = Projectile.spriteDirection;
            if (dir == -1) {
                rotation += MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }
            if (Projectile.localAI[0] > 0f) {
                drawPosition.X += Main.rand.NextFloat(-Projectile.localAI[0], Projectile.localAI[0]);
                drawPosition.Y += Main.rand.NextFloat(-Projectile.localAI[0], Projectile.localAI[0]);
            }

            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Type].Value,
                drawPosition,
                null,
                lightColor,
                rotation,
                TextureAssets.Projectile[Type].Value.Size() / 2f,
                Projectile.scale,
                effects,
                0
            );
            return false;
        }
    }
}