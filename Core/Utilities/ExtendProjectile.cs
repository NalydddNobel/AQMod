using Aequus.Common.Projectiles;
using System;
using Terraria.GameContent;

namespace Aequus.Core.Utilities;

public static class ExtendProjectile {
    internal static readonly Projectile _dummyProjectile = new Projectile();

    public static Boolean IsChildOrNoSpecialEffects(this Projectile projectile) {
        return projectile.GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects || projectile.GetGlobalProjectile<ProjectileSource>().isProjectileChild;
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

    public static Single CappedMeleeScale(this Player player) {
        var item = player.HeldItem;
        return Math.Clamp(player.GetAdjustedItemScale(item), 0.5f * item.scale, 2f * item.scale);
    }

    public static void MeleeScale(Projectile proj) {
        Single scale = Main.player[proj.owner].CappedMeleeScale();
        if (scale != 1f) {
            proj.scale *= scale;
            proj.width = (Int32)(proj.width * proj.scale);
            proj.height = (Int32)(proj.height * proj.scale);
        }
    }

    /// <summary>
    /// Attempts to find a projectile index using the identity and owner provided. Returns true if the projectile output is not -1.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="identity"></param>
    /// <param name="projectile"></param>
    /// <returns></returns>
    public static Boolean TryFindProjectileIdentity(Int32 owner, Int32 identity, out Int32 projectile) {
        projectile = FindProjectileIdentity(owner, identity);
        return projectile != -1;
    }

    /// <summary>
    /// Attempts to find a projectile index using the identity and owner provided. Returns -1 otherwise.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static Int32 FindProjectileIdentity(Int32 owner, Int32 identity) {
        for (Int32 i = 0; i < 1000; i++) {
            if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active) {
                return i;
            }
        }
        return -1;
    }
    public static Int32 FindProjectileIdentityOtherwiseFindPotentialSlot(Int32 owner, Int32 identity) {
        Int32 projectile = FindProjectileIdentity(owner, identity);
        if (projectile == -1) {
            for (Int32 i = 0; i < 1000; i++) {
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
        if (CanReflectAgainstShimmer(projectile)) {
            projectile.netUpdate = true;
        }
    }

    public static Boolean CanReflectAgainstShimmer(Entity entity) {
        if (entity.shimmerWet && entity.velocity.Y > 0f) {
            entity.velocity.Y = -entity.velocity.Y;
            return true;
        }
        return false;
    }

    #region Drawing
    public static Rectangle Frame(this Projectile projectile) {
        return TextureAssets.Projectile[projectile.type].Value.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
    }

    public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out Int32 trailLength) {
        texture = TextureAssets.Projectile[projectile.type].Value;
        offset = projectile.Size / 2f;
        frame = projectile.Frame();
        origin = frame.Size() / 2f;
        trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
    }
    #endregion
}