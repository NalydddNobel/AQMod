using Aequus.Common.Projectiles;
using Aequus.Core.Entities.Projectiles;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Core.Utilities;

public static class XProjectile {
    internal static readonly Projectile _dummyProjectile = new Projectile();

    public static bool GetItemSource(this Projectile projectile, out int itemSource, out int ammoSource) {
        if (!projectile.TryGetGlobalProjectile(out ProjectileSource source)) {
            itemSource = -1;
            ammoSource = -1;
            return false;
        }

        itemSource = source.parentItemType;
        ammoSource = source.parentAmmoType;
        return true;
    }

    public static bool AllowSpecialAbilities(this Projectile projectile) {
        if (projectile.TryGetGlobalProjectile(out ProjectileSource sources)) {
            if (sources.parentProjectileIdentity >= 0) {
                return false;
            }
        }
        if (projectile.TryGetGlobalProjectile(out ProjectileItemData itemData)) {
            if (itemData.NoSpecialEffects) {
                return false;
            }
        }
        return true;
    }

    public static IEnumerable<Projectile> Where(int owner, Predicate<Projectile> Condition) {
        for (int i = 0; i < Main.maxProjectiles; i++) {
            Projectile p = Main.projectile[i];
            if (p.active && p.owner == owner && !p.hostile && Condition(p)) {
                yield return p;
            }
        }
    }

    public static IEnumerable<Projectile> Where(Predicate<Projectile> Condition) {
        for (int i = 0; i < Main.maxProjectiles; i++) {
            Projectile p = Main.projectile[i];
            if (p.active && Condition(p)) {
                yield return p;
            }
        }
    }

    public static void CollideWithOthers(this Projectile proj, float speed = 0.05f) {
        Rectangle rect = proj.getRect();
        for (int i = 0; i < Main.maxProjectiles; i++) {
            Projectile other = Main.projectile[i];
            if (other.active && i != proj.whoAmI && proj.type == other.type
                && rect.Intersects(other.getRect())) {
                proj.velocity += (proj.Center - other.Center).SafeNormalize(Vector2.UnitY) * speed;
            }
        }
    }

    public static int GetMinionTarget(this Projectile projectile, Vector2 position, out float distance, float maxDistance = 2000f, float? ignoreTilesDistance = 0f) {
        if (Main.player[projectile.owner].HasMinionAttackTargetNPC) {
            distance = Vector2.Distance(Main.npc[Main.player[projectile.owner].MinionAttackTargetNPC].Center, projectile.Center);
            if (distance < maxDistance) {
                return Main.player[projectile.owner].MinionAttackTargetNPC;
            }
        }
        int target = -1;
        distance = maxDistance;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].CanBeChasedBy(projectile)) {
                float d = Vector2.Distance(position, Main.npc[i].Center);
                if (d < distance) {
                    if (!ignoreTilesDistance.HasValue || d < ignoreTilesDistance || Collision.CanHit(position - projectile.Size / 2f, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height)) {
                        distance = d;
                        target = i;
                    }
                }
            }
        }
        return target;
    }

    public static int GetMinionTarget(this Projectile projectile, out float distance, float maxDistance = 2000f, float? ignoreTilesDistance = 0f) {
        return projectile.GetMinionTarget(projectile.Center, out distance, maxDistance, ignoreTilesDistance);
    }

    public static bool IsChildOrNoSpecialEffects(this Projectile projectile) {
        return projectile.GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects || projectile.GetGlobalProjectile<ProjectileSource>().IsProjectileChild;
    }

    public static void SetDefaultNoInteractions(this Projectile projectile) {
        projectile.tileCollide = false;
        projectile.ignoreWater = true;
        projectile.aiStyle = -1;
        projectile.penetrate = -1;
    }

    public static void SetDefaultHeldProj(this Projectile projectile) {
        projectile.SetDefaultNoInteractions();
    }

    public static float CappedMeleeScale(this Player player) {
        var item = player.HeldItem;
        return Math.Clamp(player.GetAdjustedItemScale(item), 0.5f * item.scale, 2f * item.scale);
    }

    public static void MeleeScale(Projectile proj) {
        float scale = Main.player[proj.owner].CappedMeleeScale();
        if (scale != 1f) {
            proj.scale *= scale;
            proj.width = (int)(proj.width * proj.scale);
            proj.height = (int)(proj.height * proj.scale);
        }
    }

    /// <summary>
    /// Attempts to find a projectile index using the identity and owner provided. Returns true if the projectile output is not -1.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="identity"></param>
    /// <param name="projectile"></param>
    /// <returns></returns>
    public static bool TryFindProjectileIdentity(int owner, int identity, out int projectile) {
        projectile = FindProjectileIdentity(owner, identity);
        return projectile != -1;
    }

    /// <summary>
    /// Attempts to find a projectile index using the identity and owner provided. Returns -1 otherwise.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static int FindProjectileIdentity(int owner, int identity) {
        for (int i = 0; i < 1000; i++) {
            if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active) {
                return i;
            }
        }
        return -1;
    }
    public static int FindProjectileIdentityOtherwiseFindPotentialSlot(int owner, int identity) {
        int projectile = FindProjectileIdentity(owner, identity);
        if (projectile == -1) {
            for (int i = 0; i < 1000; i++) {
                if (!Main.projectile[i].active) {
                    projectile = i;
                    break;
                }
            }
        }
        if (projectile == 1000) {
            projectile = Projectile.FindOldestProjectile();
        }
        return projectile;
    }

    /// <summary>Helper method which reflects a projectile against shimmer.</summary>
    public static void UpdateShimmerReflection(this Projectile projectile) {
        if (ExtendEntity.CanReflectAgainstShimmer(projectile)) {
            projectile.netUpdate = true;
        }
    }


    #region Drawing
    public static Rectangle Frame(this Projectile projectile) {
        return TextureAssets.Projectile[projectile.type].Value.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
    }

    public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength) {
        texture = TextureAssets.Projectile[projectile.type].Value;
        offset = projectile.Size / 2f;
        frame = projectile.Frame();
        origin = frame.Size() / 2f;
        trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
    }
    #endregion
}