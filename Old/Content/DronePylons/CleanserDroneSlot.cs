using Aequus.DataSets;
using Aequus.Old.Content.DronePylons.NPCs;
using System.Collections.Generic;

namespace Aequus.Old.Content.DronePylons;

public class CleanserDroneSlot : DroneSlot {
    public static Dictionary<Point, int> SpecialSolutions { get; private set; }

    public override int NPCType => ModContent.NPCType<CleanserDrone>();

    public override int ItemValue => Item.buyPrice(platinum: 2);

    public override void Load() {
        SpecialSolutions = new Dictionary<Point, int>() {
            [new Point(TileID.TeleportationPylon, 0)] = ProjectileID.PureSpray,
            [new Point(TileID.TeleportationPylon, 1)] = ProjectileID.PureSpray,
            [new Point(TileID.TeleportationPylon, 2)] = ProjectileID.HallowSpray,
            [new Point(TileID.TeleportationPylon, 7)] = ProjectileID.MushroomSpray,
        };
    }

    public override void Unload() {
        SpecialSolutions?.Clear();
        SpecialSolutions = null;
    }

    public override void OnAdd(Player player) {
    }

    public override void OnHardUpdate() {
        if (GetDronePoint().isActive)
            return;

        if (Main.netMode == NetmodeID.MultiplayerClient) {
            return;
        }

        int solution = GetSolutionProjectileID();
        for (int i = 0; i < 20; i++) {
            int randX = Main.rand.Next(-50, 50);
            int randY = Main.rand.Next(-50, 50);
            var p = FindConvertibleTile(new Point(Location.X + randX, Location.Y + randY), solution);

            if (p == Point.Zero)
                return;

            if (solution > 0) {
                var spawnPosition = WorldLocation + new Vector2(randX * 16f, randY * 16f);
                var proj = Projectile.NewProjectileDirect(null, spawnPosition, Vector2.Normalize(p.ToWorldCoordinates() + new Vector2(8f) - spawnPosition) * 7.5f, solution, 0, 0, Main.myPlayer);
                proj.timeLeft *= 2;
                proj.extraUpdates = 30;
                break;
            }
        }
    }

    public int GetSolutionProjectileID() {
        var pylonStand = Location + new Point(1, 4);
        if (TileID.Sets.Hallow[Main.tile[pylonStand].TileType]) {
            return ProjectileID.HallowSpray;
        }
        if (Main.tile[pylonStand].TileType == TileID.MushroomGrass) {
            return ProjectileID.MushroomSpray;
        }
        if (TileID.Sets.Corrupt[Main.tile[pylonStand].TileType]) {
            return ProjectileID.CorruptSpray;
        }
        if (TileID.Sets.Crimson[Main.tile[pylonStand].TileType]) {
            return ProjectileID.CrimsonSpray;
        }
        if (SpecialSolutions.TryGetValue(new Point(Main.tile[Location].TileType, Main.tile[Location].TileFrameX / 54), out int id)) {
            return id;
        }
        return ProjectileID.PureSpray;
    }

    public bool CheckSolutionOverrides(Point tilePos, int solutionProj) {
        Tile tile = Main.tile[tilePos];
        ushort tileType = tile.TileType;
        var pylonStand = Location + new Point(1, 4);

        bool convertible = TileDataSet.HasAnyConversions.Contains(tileType);

        if (solutionProj == ProjectileID.MushroomSpray && tileType == TileID.JungleGrass) {
            return true;
        }
        if (solutionProj == ProjectileID.CorruptSpray || solutionProj == ProjectileID.CrimsonSpray) {
            return TileID.Sets.Hallow[tileType] || convertible;
        }
        if (solutionProj == ProjectileID.PureSpray && TileID.Sets.Hallow[tileType]) {
            return true;
        }
        if (solutionProj == ProjectileID.HallowSpray && !TileID.Sets.Hallow[tile.TileType] && convertible) {
            return true;
        }
        return TileID.Sets.Corrupt[tile.TileType] || TileID.Sets.Crimson[tile.TileType];
    }

    public Point FindConvertibleTile(Point tilePos, int solution) {
        for (int i = 0; i < 5000; i++) {
            int x = tilePos.X + Main.rand.Next(-50, 50);
            int y = tilePos.Y + Main.rand.Next(-50, 50);
            if (WorldGen.InWorld(x, y) && Main.tile[x, y].HasTile && CheckSolutionOverrides(new Point(x, y), solution)) {
                return new Point(x, y);
            }
        }
        return Point.Zero;
    }
}