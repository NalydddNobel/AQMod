using Aequus.Projectiles.Misc.Drones;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DronePylons
{
    public class CleanserDroneSlot : DroneSlot
    {
        public CleanseType cleanseType;

        public override int ProjectileType => ModContent.ProjectileType<CleanserDrone>();

        public override void OnAdd(Player player)
        {
            cleanseType = player.ZoneHallow ? CleanseType.Hallow : CleanseType.Purity;

            if (Main.tile[Location].TileFrameX / 54 == 2)
            {
                cleanseType = CleanseType.Hallow;
            }
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

                int solution = GetSolutionProjectileID(Main.tile[p].TileType);

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

        public int GetSolutionProjectileID(int tileType)
        {
            bool hallow = cleanseType == CleanseType.Hallow || (Main.tile[Location].TileFrameX / 54) == 2;
            if (hallow)
            {
                if (tileType != TileID.HallowedGrass && tileType != TileID.Pearlstone && tileType != TileID.Pearlsand
                    && tileType != TileID.HallowHardenedSand && tileType != TileID.HallowSandstone && tileType != TileID.GolfGrassHallowed
                    && tileType != TileID.HallowedIce)
                {
                    return ProjectileID.HallowSpray;
                }
            }
            else if (tileType != TileID.Grass && tileType != TileID.Stone && tileType != TileID.Sand
                && tileType != TileID.HardenedSand && tileType != TileID.Sandstone && tileType != TileID.GolfGrass
                && tileType != TileID.IceBlock && tileType != TileID.JungleThorns)
            {
                return ProjectileID.PureSpray;
            }
            return 0;
        }

        public static Point FindConvertibleTile(Point tilePos)
        {
            for (int i = 0; i < 1000; i++)
            {
                int x = tilePos.X + Main.rand.Next(-60, 60);
                int y = tilePos.Y + Main.rand.Next(-60, 60);
                if (!WorldGen.InWorld(x, y))
                {
                    continue;
                }

                if (Main.tile[x, y].HasTile)
                {
                    int type = Main.tile[x, y].TileType;
                    if (TileID.Sets.Conversion.Grass[type] || TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Sand[type]
                         || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type]
                         || TileID.Sets.Conversion.GolfGrass[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Thorn[type])
                    {
                        return new Point(x, y);
                    }
                }
            }

            return Point.Zero;
        }

        public override TagCompound SerializeData()
        {
            var tag = base.SerializeData();
            tag["CleanseType"] = (byte)cleanseType;
            return tag;
        }

        public override void DeserializeData(TagCompound tag)
        {
            base.DeserializeData(tag);
            if (tag.TryGet("CleanseType", out byte cleanseType))
            {
                this.cleanseType = (CleanseType)cleanseType;
            }
        }

        public override void SendData(BinaryWriter packet)
        {
            packet.Write((byte)cleanseType);
        }

        public override void ReceiveData(BinaryReader reader)
        {
            cleanseType = (CleanseType)reader.ReadByte();
        }
    }

    public enum CleanseType : byte
    {
        Purity = 0,
        Hallow = 1,
    }
}