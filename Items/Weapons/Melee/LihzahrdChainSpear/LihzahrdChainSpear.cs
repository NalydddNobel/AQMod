using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.LihzahrdChainSpear {
    public class LihzahrdChainSpear : ModItem {
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
            Item.shoot = ModContent.ProjectileType<LihzahrdChainSpearProj>();
        }

        public override bool CanUseItem(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool AltFunctionUse(Player player) {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 2) {
                Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    type,
                    damage,
                    knockback,
                    player.whoAmI,
                    1f
                );
                return false;
            }
            return true;
        }
    }

    public class LihzahrdChainSpearProj : HeldProjBase {
        private Asset<Texture2D> _chain;
        private int _headProj;

        public virtual Asset<Texture2D> Chain => AequusTextures.LihzahrdChainSpear_Chain;
        public virtual int HeadProj => ModContent.ProjectileType<LihzahrdChainSpearProjHead>();

        public override void SetDefaults() {
            _headProj = HeadProj;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.tileCollide = false;
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            if (player.HeldItemFixed().shoot != Type || player.itemAnimation <= 0) {
                Projectile.Kill();
                return;
            }

            Projectile.Center = player.Center;
            player.heldProj = Projectile.whoAmI;

            var v = Main.player[Projectile.owner].MountedCenter - Projectile.Center;
            armRotation = (v with { X = Math.Abs(v.X) }).ToRotation();
            SetArmRotation(player);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if ((int)Projectile.ai[0] == 1) {
                if ((int)Projectile.ai[1] == 0) {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        Projectile.velocity * 15f,
                        _headProj,
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                    Projectile.ai[1] = 1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Main.instance.LoadProjectile(_headProj);
            _chain ??= Chain;

            var player = Main.player[Projectile.owner];
            var playerCenter = (player.Center + new Vector2(0f, player.gfxOffY)).Floor();
            var texture = TextureAssets.Projectile[Type].Value;
            var headTexture = TextureAssets.Projectile[_headProj].Value;
            float rotation = Projectile.rotation + MathHelper.PiOver4;
            float headOffsetRotation = rotation - MathHelper.PiOver4;
            var effects = SpriteEffects.None;

            var dir = Math.Sign(Projectile.velocity.X);
            if (dir == -1) {
                effects = SpriteEffects.FlipHorizontally;
                rotation += MathHelper.PiOver2;
            }

            Main.EntitySpriteDraw(
                texture,
                playerCenter - Main.screenPosition,
                null,
                lightColor,
                rotation,
                texture.Size() / 2f,
                Projectile.scale,
                effects,
                0
            );

            if ((int)Projectile.ai[0] != 1) {
                Main.EntitySpriteDraw(
                    headTexture,
                    (playerCenter + headOffsetRotation.ToRotationVector2() * (texture.Size().Length() / 2f + 16f) * Projectile.scale - Main.screenPosition + new Vector2(2f, 0f)).Floor(),
                    null,
                    lightColor,
                    rotation,
                    headTexture.Size() / 2f,
                    Projectile.scale,
                    effects,
                    0
                );
            }
            return false;
        }
    }

    public class LihzahrdChainSpearProjHead : ModProjectile {
        public override void SetDefaults() {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI() {
            Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.Torch
            );
            var player = Main.player[Projectile.owner];
            player.itemAnimation = 2;
            player.itemTime = 2;
            float distance = Projectile.Distance(player.Center);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if ((int)Projectile.ai[0] == 1) {
                if (distance < 80f) {
                    Projectile.Kill();
                }
                Projectile.rotation -= MathHelper.Pi;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * 40f, 0.33f);
                return;
            }
            if (distance > 500f) {
                Projectile.ai[0] = 1f;
                Projectile.velocity = -Projectile.velocity;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 16;
            height = 16;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.ai[0] = 1f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            float rotation = Projectile.rotation + MathHelper.PiOver4;
            var effects = SpriteEffects.None;
            var dir = Math.Sign(Projectile.velocity.X);
            if ((int)Projectile.ai[0] == 1) {
                dir = -dir;
            }
            if (dir == -1) {
                rotation += MathHelper.PiOver2;
                effects = SpriteEffects.FlipHorizontally;
            }

            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Type].Value,
                Projectile.Center - Main.screenPosition,
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