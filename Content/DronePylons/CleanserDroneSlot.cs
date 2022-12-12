using Aequus.NPCs.Friendly.Drones;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons
{
    public class CleanserDroneSlot : DroneSlot
    {
        public static Dictionary<Point, int> SpecialSolutions { get; private set; }

        public override int NPCType => ModContent.NPCType<CleanserDrone>();

        public override void Load()
        {
            SpecialSolutions = new Dictionary<Point, int>()
            {
                [new Point(TileID.TeleportationPylon, 2)] = ProjectileID.HallowSpray,
            };
        }

        public override void Unload()
        {
            SpecialSolutions?.Clear();
            SpecialSolutions = null;
        }

        public override void OnAdd(Player player)
        {
        }

        public override void OnHardUpdate()
        {
            if (GetDronePoint().isActive)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            for (int i = 0; i < 20; i++)
            {
                var p = FindConvertibleTile(Location);

                if (p == Point.Zero)
                    return;

                int solution = GetSolutionProjectileID(p);

                if (solution > 0)
                {
                    var spawnPosition = WorldLocation;
                    var proj = Projectile.NewProjectileDirect(null, spawnPosition, Vector2.Normalize(p.ToWorldCoordinates() + new Vector2(8f) - spawnPosition) * 7.5f, solution, 0, 0, Main.myPlayer);
                    proj.timeLeft *= 2;
                    proj.extraUpdates = 30;
                    break;
                }
            }
        }

        public int GetSolutionProjectileID(Point tilePos)
        {
            var tile = Main.tile[tilePos];
            if (SpecialSolutions.TryGetValue(new Point(Main.tile[Location].TileType, Main.tile[Location].TileFrameX / 54), out int id))
            {
                return id;
            }
            var pylonStand = Location + new Point(1, 4);
            AequusHelpers.dustDebug(pylonStand);
            if (TileID.Sets.Hallow[Main.tile[pylonStand].TileType])
            {
                return ProjectileID.HallowSpray;
            }
            if (Main.tile[pylonStand].TileType == TileID.MushroomGrass)
            {
                return ProjectileID.MushroomSpray;
            }
            if (TileID.Sets.Corrupt[Main.tile[pylonStand].TileType])
            {
                return ProjectileID.CorruptSpray;
            }
            if (TileID.Sets.Crimson[Main.tile[pylonStand].TileType])
            {
                return ProjectileID.CrimsonSpray;
            }
            return ProjectileID.PureSpray;
        }

        public bool ShouldCleanse(Point tilePos)
        {
            var tile = Main.tile[tilePos];
            var pylonStand = Location + new Point(1, 4);
            if (Main.tile[pylonStand].TileType == TileID.MushroomGrass)
            {
                return Main.tile[pylonStand].TileType == TileID.JungleGrass;
            }
            if (TileID.Sets.Corrupt[Main.tile[pylonStand].TileType] || TileID.Sets.Crimson[Main.tile[pylonStand].TileType])
            {
                return TileID.Sets.Hallow[Main.tile[pylonStand].TileType] || TileID.Sets.Conversion.Grass[Main.tile[pylonStand].TileType]
                    || TileID.Sets.Conversion.Stone[Main.tile[pylonStand].TileType] || TileID.Sets.Conversion.Sand[Main.tile[pylonStand].TileType]
                    || TileID.Sets.Conversion.Ice[Main.tile[pylonStand].TileType];
            }
            return TileID.Sets.Corrupt[tile.TileType] || TileID.Sets.Crimson[tile.TileType];
        }

        public Point FindConvertibleTile(Point tilePos)
        {
            for (int i = 0; i < 1000; i++)
            {
                int x = tilePos.X + Main.rand.Next(-60, 60);
                int y = tilePos.Y + Main.rand.Next(-60, 60);
                if (WorldGen.InWorld(x, y) && Main.tile[x, y].HasTile && ShouldCleanse(tilePos))
                {
                    return new Point(x, y);
                }
            }
            return Point.Zero;
        }
    }
}