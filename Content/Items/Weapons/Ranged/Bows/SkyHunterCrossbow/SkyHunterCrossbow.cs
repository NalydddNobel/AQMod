using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;

public class SkyHunterCrossbow : ModItem, IManageProjectile {
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
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem + 1;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-10f, 0f);
    }

    #region Projectiles
    private void ProjectileTryPickupItems(Projectile projectile) {
        var hitbox = Utils.CenteredRectangle(projectile.Center, new Vector2(64f, 64f));
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

    public bool PreAIProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
        if (aequusProjectile.isProjectileChild) {
            return true;
        }
        if (aequusProjectile.noSpecialEffects) {
            if (aequusProjectile.itemData != 0) {
                BreakRopeParticles(projectile, 14f, 3f);
                aequusProjectile.itemData = 0;
            }
            return true;
        }
        if (aequusProjectile.itemData == 0) {
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
        float distance = projectile.Distance(Main.player[projectile.owner].Center);
        if (aequusProjectile.itemData >= 0) {
            aequusProjectile.itemData++;
            ProjectileTryPickupItems(projectile);
            if (projectile.penetrate == -1) {
                aequusProjectile.itemData = Math.Max(aequusProjectile.itemData, 1000 - 4);
            }
            if (distance > 600f || aequusProjectile.itemData > 1000) {
                aequusProjectile.itemData = -1;
            }
            if (aequusProjectile.itemData > 990) {
                projectile.tileCollide = false;
            }
            return true;
        }

        var difference = Main.player[projectile.owner].Center - projectile.Center;
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
            if (aequusProjectile.itemData == -1) {
                projectile.velocity /= projectile.MaxUpdates;
                projectile.netUpdate = true;
            }
            if (aequusProjectile.itemData == -2) {
                SoundEngine.PlaySound(AequusSounds.RopeRetract with { Volume = 1.2f, PitchVariance = 0.2f }, projectile.Center);
            }
            aequusProjectile.itemData--;
            projectile.rotation = projectile.rotation.AngleTowards(arrowRotationVector.ToRotation(), 0.02f);
            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * speed, MathF.Min(-aequusProjectile.itemData * 0.01f, 1f));
        }
        if (distance < 32f) {
            if (Main.myPlayer == projectile.owner) {
                if (aequusProjectile.parentAmmoType > 0 && ContentSamples.ItemsByType[aequusProjectile.parentAmmoType].consumable) {
                    var item = new Item(aequusProjectile.parentAmmoType);
                    item = Main.player[projectile.owner].GetItem(projectile.owner, item, GetItemSettings.PickupItemFromWorld);
                    if (item != null && !item.IsAir) {
                        int newItemIndex = Item.NewItem(projectile.GetSource_FromThis(), Main.player[projectile.owner].getRect(), item);
                        Main.item[newItemIndex].newAndShiny = false;
                        if (Main.netMode == NetmodeID.MultiplayerClient) {
                            NetMessage.SendData(MessageID.SyncItem, number: newItemIndex, number2: 1f);
                        }
                    }
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

    public bool PreDrawProjectile(Projectile projectile, AequusProjectile aequusProjectile, ref Color lightColor) {
        if (aequusProjectile.IsChildOrNoSpecialEffects) {
            return true;
        }

        var chainTexture = AequusTextures.Chain;
        var projectileCenter = projectile.Center;
        var chainPosition = Main.player[projectile.owner].MountedCenter;
        var difference = projectileCenter - chainPosition;
        var chainVelocity = Vector2.Normalize(difference) * chainTexture.Height;
        var chainOrigin = chainTexture.Size() / 2f;
        float minDistance = MathF.Pow(chainTexture.Height, 2);
        float opacity = projectile.Opacity;
        var shakeVector = Vector2.Normalize(chainVelocity).RotatedBy(MathHelper.PiOver2);
        var random = new FastRandom(Main.player[projectile.owner].name.GetHashCode());
        float intensity = 0f;
        if (projectile.timeLeft < 30) {
            intensity = (30 - projectile.timeLeft) / 30f;
        }
        if (aequusProjectile.itemData > 0 && aequusProjectile.itemData < 40) {
            intensity = Math.Max(intensity, (40f - aequusProjectile.itemData) / 40f);
        }
        for (int i = 0; i < 200; i++) {
            var offset = Vector2.Zero;
            float rotation = chainVelocity.ToRotation();
            float chainDistance = Vector2.Distance(chainPosition, projectile.Center);
            if (intensity > 0f && chainDistance > 8f) {
                float shakeMultiplier = Math.Min((chainDistance - 8f) / 200f, 1f);
                float shakeValue = MathF.Sin(Main.GlobalTimeWrappedHourly * 33f + i * 0.3f) * intensity;
                offset += shakeVector * shakeValue * 12f * shakeMultiplier;
                rotation -= shakeValue * 0.4f;
            }
            Main.EntitySpriteDraw(chainTexture, chainPosition - Main.screenPosition + offset, null, Lighting.GetColor(chainPosition.ToTileCoordinates()) * opacity, rotation, chainOrigin, 1f, SpriteEffects.None);
            chainPosition += chainVelocity;
            if (Vector2.DistanceSquared(chainPosition, projectileCenter) < minDistance) {
                break;
            }
        }
        return true;
    }

    public bool OnTileCollideProjectile(Projectile projectile, AequusProjectile aequusProjectile, Vector2 oldVelocity) {
        if (aequusProjectile.IsChildOrNoSpecialEffects) {
            return true;
        }
        //if (projectile.type == ProjectileID.ChlorophyteArrow) {
        //    return true;
        //}
        //aequusProjectile.itemData = -1;
        //projectile.tileCollide = false;
        //projectile.velocity = oldVelocity;
        //return false;
        return true;
    }

    public void OnHitNPCProjectile(Projectile projectile, AequusProjectile aequusProjectile, NPC target, NPC.HitInfo hit, int damageDone) {
        if (aequusProjectile.IsChildOrNoSpecialEffects) {
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

    public void OnKillProjectile(Projectile projectile, AequusProjectile aequusProjectile, int timeLeft) {
        if (Main.dedServ || aequusProjectile.itemData <= -3 || aequusProjectile.IsChildOrNoSpecialEffects) {
            return;
        }

        BreakRopeParticles(projectile, 14f, 3f);
    }

    public static void BreakRopeParticles(Projectile projectile, float ropeSegmentDistance, float ropeSegmentSize) {
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
    #endregion
}