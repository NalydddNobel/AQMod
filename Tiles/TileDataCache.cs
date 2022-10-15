using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles
{
    public struct TileDataCache
    {
        internal class TileReflectionHelper : ILoadable
        {
            public static FieldInfo LiquidData_typeAndFlags;
            public static FieldInfo TileWallWireStateData_bitpack;

            void ILoadable.Load(Mod mod)
            {
                LiquidData_typeAndFlags = typeof(LiquidData).GetField("typeAndFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                TileWallWireStateData_bitpack = typeof(TileWallWireStateData).GetField("bitpack", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            void ILoadable.Unload()
            {
            }
        }

        public readonly TileTypeData Type;
        public readonly WallTypeData Wall;
        public readonly LiquidData Liquid;
        public readonly TileWallWireStateData Misc;
        public readonly AequusTileData Aequus;

        public ushort TileType => Type.Type;
        public bool HasTile => Misc.HasTile;
        public bool IsSolid => Main.tileSolid[TileType];
        public bool IsSolidTop => Main.tileSolidTop[TileType];
        public bool IsFullySolid => HasTile && IsSolid;
        public bool IsHalfBlock => Misc.IsHalfBlock;
        public SlopeType Slope => Misc.Slope;
        public short TileFrameX => Misc.TileFrameX;
        public short TileFrameY => Misc.TileFrameY;
        public ushort TileColor => Misc.TileColor;
        public ushort WallColor => Misc.WallColor;
        public byte LiquidAmount => Liquid.Amount;
        public ushort WallType => Wall.Type;

        public TileDataCache(TileTypeData type, LiquidData liquid, TileWallWireStateData misc, WallTypeData wall, AequusTileData aequus)
        {
            Type = type;
            Liquid = liquid;
            Misc = misc;
            Wall = wall;
            Aequus = aequus;
        }

        public TileDataCache(Tile tile) : this(tile.Get<TileTypeData>(), tile.Get<LiquidData>(), tile.Get<TileWallWireStateData>(), tile.Get<WallTypeData>(), tile.Get<AequusTileData>())
        {
        }

        public void Set(Tile tile)
        {
            tile.Get<TileTypeData>() = Type;
            tile.Get<LiquidData>() = Liquid;
            tile.Get<TileWallWireStateData>() = Misc;
            tile.Get<WallTypeData>() = Wall;
            tile.Get<AequusTileData>() = Aequus;
        }
    }
}