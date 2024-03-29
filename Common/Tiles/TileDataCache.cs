﻿using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles {
    public struct TileDataCache {
        internal class TileReflectionHelper : ILoadable {
            public static FieldInfo LiquidData_typeAndFlags;
            public static FieldInfo TileWallWireStateData_bitpack;

            void ILoadable.Load(Mod mod) {
                LiquidData_typeAndFlags = typeof(LiquidData).GetField("typeAndFlags", BindingFlags.NonPublic | BindingFlags.Instance);
                TileWallWireStateData_bitpack = typeof(TileWallWireStateData).GetField("bitpack", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            void ILoadable.Unload() {
            }
        }

        public readonly TileTypeData Type;
        public readonly WallTypeData Wall;
        public readonly LiquidData Liquid;
        public readonly TileWallWireStateData Misc;
        public readonly TileWallBrightnessInvisibilityData CoatingData;

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
        public bool HasActuator => Misc.HasActuator;
        public int WireData => Misc.WireData;
        public byte LiquidAmount => Liquid.Amount;
        public int LiquidType => Liquid.LiquidType;
        public ushort WallType => Wall.Type;

        public void SetTile(Tile tile) {
            tile.Get<TileTypeData>() = Type;
            tile.Get<LiquidData>() = Liquid;
            tile.Get<TileWallWireStateData>() = Misc;
            tile.Get<WallTypeData>() = Wall;
            tile.Get<TileWallBrightnessInvisibilityData>() = CoatingData;
        }

        public TileDataCache(TileTypeData type, LiquidData liquid, TileWallWireStateData misc, WallTypeData wall, TileWallBrightnessInvisibilityData coatingData) {
            Type = type;
            Liquid = liquid;
            Misc = misc;
            Wall = wall;
            CoatingData = coatingData;
        }

        public TileDataCache(Tile tile) : this(tile.Get<TileTypeData>(), tile.Get<LiquidData>(), tile.Get<TileWallWireStateData>(), tile.Get<WallTypeData>(), tile.Get<TileWallBrightnessInvisibilityData>()) {
        }

        public void Set(Tile tile) {
            tile.Get<TileTypeData>() = Type;
            tile.Get<LiquidData>() = Liquid;
            tile.Get<TileWallWireStateData>() = Misc;
            tile.Get<WallTypeData>() = Wall;
            tile.Get<TileWallBrightnessInvisibilityData>() = CoatingData;
        }
    }
}