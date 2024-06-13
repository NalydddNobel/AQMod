using System;

namespace Aequus.Core.Utilities;

public static partial class Helper {
    [Obsolete]
    public static int FixedDamage(this NPC npc) {
        return Main.masterMode ? npc.damage / 3 : Main.expertMode ? npc.damage / 2 : npc.damage;
    }

    public static void SetIDStaticHitCooldown(this NPC npc, int projID, uint time) {
        Projectile.perIDStaticNPCImmunity[projID][npc.whoAmI] = Main.GameUpdateCount + time;
    }
    public static void SetIDStaticHitCooldown<T>(this NPC npc, uint time) where T : ModProjectile {
        SetIDStaticHitCooldown(npc, ModContent.ProjectileType<T>(), time);
    }

    [Obsolete("Use the \"with { A = <value> }\" expression.")]
    public static Color UseA(this Color rgba, byte A) {
        return rgba with { A = A };
    }

    [Obsolete]
    public static void SetTrail(this ModProjectile modProjectile, int length = -1) {
        if (length > 0) {
            ProjectileID.Sets.TrailCacheLength[modProjectile.Type] = length;
        }
        ProjectileID.Sets.TrailingMode[modProjectile.Type] = 2;
    }

    [Obsolete]
    public static AequusPlayer Aequus(this Player player) {
        return player.GetModPlayer<AequusPlayer>();
    }

    [Obsolete]
    public static void UpdateCacheList<T>(T[] arr) {
        for (int i = arr.Length - 1; i > 0; i--) {
            arr[i] = arr[i - 1];
        }
    }

    [Obsolete]
    public static bool ShadedSpot(int x, int y) {
        if (!WorldGen.InWorld(x, y) || y > (int)Main.worldSurface || Main.tile[x, y].IsFullySolid()) {
            return true;
        }

        for (int j = 1; j < 10; j++) {
            if (!WorldGen.InWorld(x, y - 1, 10)) {
                break;
            }

            if (Main.tile[x, y - j].HasTile
                && !Main.tileSolidTop[Main.tile[x, y - j].TileType]
                && Main.tileSolid[Main.tile[x, y - j].TileType]) {
                return true;
            }
        }

        return Main.tile[x, y].WallType == WallID.None && !WallID.Sets.Transparent[Main.tile[x, y].WallType] && !WallID.Sets.AllowsWind[Main.tile[x, y].WallType];
    }
    [Obsolete]
    public static bool ShadedSpot(Point tileCoordinates) {
        return ShadedSpot(tileCoordinates.X, tileCoordinates.Y);
    }
    [Obsolete]
    public static bool ShadedSpot(Vector2 worldCoordinates) {
        return ShadedSpot(worldCoordinates.ToTileCoordinates());
    }
}
