using Terraria.DataStructures;

namespace Aequus.Old.Core.Utilities;
public static class OldHelper {
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
        return GetMinionTarget(projectile, projectile.Center, out distance, maxDistance, ignoreTilesDistance);
    }

    public static void DropHearts(IEntitySource source, Rectangle hitbox, int guaranteedAmount, int randomAmount) {
        for (int i = 0; i < guaranteedAmount; i++) {
            Item.NewItem(source, hitbox, ItemID.Heart);
        }
        randomAmount = Main.rand.Next(randomAmount);
        for (int i = 0; i < randomAmount; i++) {
            Item.NewItem(source, hitbox, ItemID.Heart);
        }
    }

    public static void UpdateCacheList<T>(T[] arr) {
        for (int i = arr.Length - 1; i > 0; i--) {
            arr[i] = arr[i - 1];
        }
    }

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
    public static bool ShadedSpot(Point tileCoordinates) {
        return ShadedSpot(tileCoordinates.X, tileCoordinates.Y);
    }
    public static bool ShadedSpot(Vector2 worldCoordinates) {
        return ShadedSpot(worldCoordinates.ToTileCoordinates());
    }
}
