using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content
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
        public readonly LiquidData Liquid;
        public readonly TileWallWireStateData Misc;

        public ushort TileType => Type.Type;
        public bool HasTile => Misc.HasTile;
        public bool IsSolid => Main.tileSolid[TileType];
        public bool IsFullySolid => HasTile && Main.tileSolid[TileType];
        public bool IsHalfBlock => Misc.IsHalfBlock;
        public SlopeType Slope => Misc.Slope;
        public byte LiquidAmount => Liquid.Amount;


        public ushort LiquidPacked => MergeBytes(Liquid.Amount, TileReflectionHelper.LiquidData_typeAndFlags.GetValue<byte>(Liquid));
        public long MiscPacked => MergeInts(MergeShorts(Misc.TileFrameX, Misc.TileFrameY), TileReflectionHelper.TileWallWireStateData_bitpack.GetValue<byte>(Misc));

        public TileDataCache(TileTypeData type, LiquidData liquid, TileWallWireStateData misc)
        {
            Type = type;
            Liquid = liquid;
            Misc = misc;
        }

        public TileDataCache(Tile tile) : this(tile.Get<TileTypeData>(), tile.Get<LiquidData>(), tile.Get<TileWallWireStateData>())
        {
        }

        public void Set(Tile tile)
        {
            tile.Get<TileTypeData>() = Type;
            tile.Get<LiquidData>() = Liquid;
            tile.Get<TileWallWireStateData>() = Misc;
        }

        public ushort MergeBytes(byte byte1, byte byte2)
        {
            return (ushort)(byte1 + byte2 * byte.MaxValue);
        }

        public int MergeShorts(short short1, short short2)
        {
            return short1 + short2 * ushort.MaxValue;
        }

        public long MergeInts(int int1, int int2)
        {
            return int1 + (long)int2 * int.MaxValue;
        }
    }
}