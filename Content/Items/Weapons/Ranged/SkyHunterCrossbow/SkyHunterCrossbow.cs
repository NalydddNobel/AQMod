using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Projectiles;
using Aequus.Projectiles;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Aequus.Content.Items.Weapons.Ranged.SkyHunterCrossbow;

public class SkyHunterCrossbow : ModItem, IManageProjectile {
    public static int ItemPickupHitSquareSize { get; set; } = 64;
    public static int MaximumDistance { get; set; } = 600;

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.SetWeaponValues(40, 3f);
        Item.DamageType = DamageClass.Ranged;
        Item.useAmmo = AmmoID.Arrow;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 50;
        Item.useTime = 50;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.UseSound = AequusSounds.CrossbowShoot with { Volume = 0.6f, PitchVariance = 0.2f };
        Item.shootSpeed = 16f;
        Item.noMelee = true;
        Item.rare = ItemRarityID.Green;
        Item.value = ItemDefaults.NPCSkyMerchant;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-10f, 0f);
    }

    #region Ammo Effects
    internal static void DrawChain(Vector2 startPosition, Vector2 endPosition, float opacity, int animationTimer, int randomSeed, int projectileTimeLeft = int.MaxValue) {
        var chainTexture = AequusTextures.SkyHunterCrossbowChain;
        var difference = startPosition - endPosition;
        var chainVelocity = Vector2.Normalize(difference) * chainTexture.Height();
        var chainOrigin = chainTexture.Size() / 2f;
        float minDistance = MathF.Pow(chainTexture.Height(), 2);
        var shakeVector = Vector2.Normalize(chainVelocity).RotatedBy(MathHelper.PiOver2);
        var random = new FastRandom(randomSeed);
        float intensity = 0f;
        if (projectileTimeLeft < 30) {
            intensity = (30 - projectileTimeLeft) / 30f;
        }
        if (animationTimer > 0 && animationTimer < 40) {
            intensity = Math.Max(intensity, (40f - animationTimer) / 40f);
        }
        for (int i = 0; i < 200; i++) {
            var offset = Vector2.Zero;
            float rotation = chainVelocity.ToRotation();
            float chainDistance = Vector2.Distance(endPosition, startPosition);
            if (intensity > 0f && chainDistance > 8f) {
                float shakeMultiplier = Math.Min((chainDistance - 8f) / 200f, 1f);
                float shakeValue = MathF.Sin(Main.GlobalTimeWrappedHourly * 33f + i * 0.3f) * intensity;
                offset += shakeVector * shakeValue * 12f * shakeMultiplier;
                rotation -= shakeValue * 0.4f;
            }
            Main.EntitySpriteDraw(chainTexture, endPosition - Main.screenPosition + offset, null, Lighting.GetColor(endPosition.ToTileCoordinates()) * opacity, rotation, chainOrigin, 1f, SpriteEffects.None);
            endPosition += chainVelocity;
            if (Vector2.DistanceSquared(endPosition, startPosition) < minDistance) {
                break;
            }
        }
    }

    static void ProjectileTryPickupItems(Projectile projectile) {
        var hitbox = Utils.CenteredRectangle(projectile.Center, new Vector2(ItemPickupHitSquareSize));
        for (int i = 0; i < Main.maxItems; i++) {
            if (Main.item[i] != null && Main.item[i].active && Main.item[i].noGrabDelay <= 0 && Main.player[projectile.owner].ItemSpace(Main.item[i]).CanTakeItem && Main.item[i].Hitbox.Intersects(hitbox)) {
                Main.item[i].Center = Main.player[projectile.owner].Center;
                Main.item[i].velocity = Main.player[projectile.owner].velocity;
                Main.timeItemSlotCannotBeReusedFor[i] = 2;
                if (!Main.item[i].instanced && Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, number: i);
                }
            }
        }
    }

    static void BreakRopeParticles(Projectile projectile, float ropeSegmentDistance, float ropeSegmentSize) {
        var projectileCenter = projectile.Center;
        var chainPosition = Main.player[projectile.owner].MountedCenter;
        var chainDifference = projectileCenter - chainPosition;
        var chainVelocity = Vector2.Normalize(chainDifference) * ropeSegmentDistance;
        for (int i = 0; i < 200; i++) {
            var d = Dust.NewDustPerfect(chainPosition + Main.rand.NextVector2Square(-ropeSegmentSize, ropeSegmentSize), DustID.Rope);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.2f;
            d.velocity *= Main.rand.NextFloat(1f);
            chainPosition += chainVelocity;
            if (Vector2.DistanceSquared(chainPosition, projectileCenter) < 1024f) {
                break;
            }
        }
    }

    bool IManageProjectile.PreAIProjectile(Projectile projectile) {
        AequusProjectile projectileSource = projectile.GetGlobalProjectile<AequusProjectile>();
        ItemControl aequusProjectile = projectile.GetGlobalProjectile<ItemControl>();
        if (projectileSource.HasProjectileOwner) {
            return true;
        }
        if (aequusProjectile.NoSpecialEffects) {
            if (aequusProjectile.ItemAI != 0) {
                BreakRopeParticles(projectile, 14f, 3f);
                aequusProjectile.ItemAI = 0;
            }
            return true;
        }
        if (aequusProjectile.ItemAI == 0) {
            if (projectile.penetrate == -1) {
                projectile.penetrate = -2;
            }
        }
        //projectile.ignoreWater = true;
        projectile.stopsDealingDamageAfterPenetrateHits = true;
        //projectile.penetrate = Math.Min(projectile.penetrate, -1);
        if (!projectile.usesIDStaticNPCImmunity && !projectile.usesIDStaticNPCImmunity) {
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 20;
        }
        var difference = Main.player[projectile.owner].Center - projectile.Center;
        float distance = difference.Length();
        if (aequusProjectile.ItemAI >= 0) {
            aequusProjectile.ItemAI++;
            ProjectileTryPickupItems(projectile);
            if (projectile.penetrate == -1) {
                aequusProjectile.ItemAI = Math.Max(aequusProjectile.ItemAI, 1000 - 4);
            }
            if (distance > MaximumDistance || aequusProjectile.ItemAI > 1000) {
                aequusProjectile.ItemAI = -1;
            }
            if (aequusProjectile.ItemAI > 990) {
                projectile.tileCollide = false;
            }
            return true;
        }

        float speed = Math.Max(Main.player[projectile.owner].velocity.Length() * 2f, 60f) / projectile.MaxUpdates;
        projectile.friendly = false;
        projectile.hostile = false;
        projectile.extraUpdates = 4;
        projectile.timeLeft = Math.Clamp(projectile.timeLeft, 22, 44);
        projectile.tileCollide = false;
        var arrowRotationVector = difference;
        if (!Main.dedServ) {
            var texture = TextureAssets.Projectile[projectile.type].Value;
            if (texture.Height > texture.Width) {
                arrowRotationVector = arrowRotationVector.RotatedBy(-MathHelper.PiOver2);
            }
        }
        if (projectile.numUpdates != -1) {
            projectile.timeLeft++;
        }
        else {
            if (aequusProjectile.ItemAI == -1) {
                projectile.velocity /= projectile.MaxUpdates;
                projectile.netUpdate = true;
            }
            if (aequusProjectile.ItemAI == -2) {
                SoundEngine.PlaySound(AequusSounds.RopeRetract with { Volume = 1.2f, PitchVariance = 0.2f }, projectile.Center);
            }
            aequusProjectile.ItemAI--;
            projectile.rotation = projectile.rotation.AngleTowards(arrowRotationVector.ToRotation(), 0.02f);
            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * speed, MathF.Min(-aequusProjectile.ItemAI * 0.01f, 1f));
        }
        if (distance < 32f) {
            if (Main.myPlayer == projectile.owner) {
                if (projectileSource.sourceAmmoUsed > 0 && ContentSamples.ItemsByType[projectileSource.sourceAmmoUsed].consumable) {
                    Main.player[projectile.owner].GiveItem(
                        source: projectile.GetSource_FromThis(),
                        type: projectileSource.sourceAmmoUsed,
                        stack: 1,
                        getItemSettings: GetItemSettings.PickupItemFromWorld);
                }
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendData(MessageID.KillProjectile, -1, -1, null, projectile.identity, projectile.owner);
                }
            }
            projectile.active = false;
            SoundEngine.PlaySound(AequusSounds.CrossbowReload with { Volume = 0.8f, PitchVariance = 0.1f }, projectile.Center);
            if (Main.myPlayer == projectile.owner) {
                SoundEngine.PlaySound(SoundID.Grab, projectile.Center);
            }
        }
        return false;
    }


    bool IManageProjectile.PreDrawProjectile(Projectile projectile, ref Color lightColor) {
        if (projectile.IsChildOrNoSpecialEffects()) {
            return true;
        }

        DrawChain(projectile.Center, Main.player[projectile.owner].MountedCenter, projectile.Opacity, projectile.GetGlobalProjectile<ItemControl>().ItemAI, Main.player[projectile.owner].name.GetHashCode(), projectile.timeLeft);
        return true;
    }

    bool IManageProjectile.OnTileCollideProjectile(Projectile projectile, Vector2 oldVelocity) {
        //if (aequusProjectile.IsChildOrNoSpecialEffects) {
        //    return true;
        //}
        //if (projectile.type == ProjectileID.ChlorophyteArrow) {
        //    return true;
        //}
        //aequusProjectile.itemData = -1;
        //projectile.tileCollide = false;
        //projectile.velocity = oldVelocity;
        //return false;
        return true;
    }

    void IManageProjectile.OnHitNPCProjectile(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        if (projectile.IsChildOrNoSpecialEffects()) {
            return;
        }

        if (projectile.penetrate == 1) {
            if (Main.myPlayer == projectile.owner) {
                int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                Main.projectile[p].localAI[0] = projectile.localAI[0];
                Main.projectile[p].localAI[1] = projectile.localAI[1];
                Main.projectile[p].localAI[2] = projectile.localAI[2];
                Main.projectile[p].Kill();
            }
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = false;
        }
    }

    void IManageProjectile.OnKillProjectile(Projectile projectile, int timeLeft) {
        if (Main.dedServ || projectile.GetGlobalProjectile<ItemControl>().ItemAI <= -3 || projectile.IsChildOrNoSpecialEffects()) {
            return;
        }

        BreakRopeParticles(projectile, 14f, 3f);
    }
    #endregion
}