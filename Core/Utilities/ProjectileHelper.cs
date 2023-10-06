using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus;

public static class ProjectileHelper {
    public static void SetDefaultNoInteractions(this Projectile projectile) {
        projectile.tileCollide = false;
        projectile.ignoreWater = true;
        projectile.aiStyle = -1;
        projectile.penetrate = -1;
    }

    public static void SetDefaultHeldProj(this Projectile projectile) {
        SetDefaultNoInteractions(projectile);
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