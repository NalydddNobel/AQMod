using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Base {
    public abstract class BaseGemTile : ModTile {
        protected enum GemAnchor {
            AnchorTop,
            AnchorBottom,
            AnchorLeft,
            AnchorRight,
        }

        protected virtual void ModifyObjectData(GemAnchor type, TileObjectData objectData) {
        }

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = Point16.Zero;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16
            };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.DrawXOffset = 0;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            ModifyObjectData(GemAnchor.AnchorTop, TileObjectData.newTile);

            TileObjectData.newAlternate.Width = 1;
            TileObjectData.newAlternate.Height = 1;
            TileObjectData.newAlternate.Origin = Point16.Zero;
            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.CoordinateHeights = new int[]
            {
                16
            };
            TileObjectData.newAlternate.CoordinateWidth = 16;
            TileObjectData.newAlternate.CoordinatePadding = 2;
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newAlternate.LavaDeath = false;
            TileObjectData.newAlternate.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.RandomStyleRange = 3;
            TileObjectData.newAlternate.StyleMultiplier = 3;
            TileObjectData.newAlternate.DrawXOffset = 0;
            TileObjectData.newAlternate.DrawYOffset = 2;
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidWithTop | AnchorType.Table, 1, 0);
            ModifyObjectData(GemAnchor.AnchorBottom, TileObjectData.newAlternate);
            TileObjectData.addAlternate(3);

            TileObjectData.newAlternate.Width = 1;
            TileObjectData.newAlternate.Height = 1;
            TileObjectData.newAlternate.Origin = Point16.Zero;
            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.CoordinateHeights = new int[]
            {
                16
            };
            TileObjectData.newAlternate.CoordinateWidth = 16;
            TileObjectData.newAlternate.CoordinatePadding = 2;
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newAlternate.LavaDeath = false;
            TileObjectData.newAlternate.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.RandomStyleRange = 3;
            TileObjectData.newAlternate.DrawXOffset = -2;
            TileObjectData.newAlternate.DrawYOffset = 0;
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            ModifyObjectData(GemAnchor.AnchorLeft, TileObjectData.newAlternate);
            TileObjectData.addAlternate(6);

            TileObjectData.newAlternate.Width = 1;
            TileObjectData.newAlternate.Height = 1;
            TileObjectData.newAlternate.Origin = Point16.Zero;
            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.CoordinateHeights = new int[]
            {
                16
            };
            TileObjectData.newAlternate.CoordinateWidth = 16;
            TileObjectData.newAlternate.CoordinatePadding = 2;
            TileObjectData.newAlternate.StyleHorizontal = true;
            TileObjectData.newAlternate.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newAlternate.LavaDeath = false;
            TileObjectData.newAlternate.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.RandomStyleRange = 3;
            TileObjectData.newAlternate.DrawXOffset = 2;
            TileObjectData.newAlternate.DrawYOffset = 0;
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);
            ModifyObjectData(GemAnchor.AnchorRight, TileObjectData.newAlternate);
            TileObjectData.addAlternate(9);

            TileObjectData.addTile(Type);
        }

        public override bool CanPlace(int i, int j) {
            var tile = Framing.GetTileSafely(i, j);
            var anchorTile = Framing.GetTileSafely(i, j - 1);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorTop(i, j, tile, anchorTile)) {
                return true;
            }

            anchorTile = Framing.GetTileSafely(i, j + 1);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorBottom(i, j, tile, anchorTile)) {
                return true;
            }

            anchorTile = Framing.GetTileSafely(i - 1, j);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorLeft(i, j, tile, anchorTile)) {
                return true;
            }

            anchorTile = Framing.GetTileSafely(i + 1, j);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorRight(i, j, tile, anchorTile)) {
                return true;
            }

            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
            GemFrame(i, j);
            return false;
        }

        public virtual bool CanAnchorTop(int i, int j, Tile tile, Tile top) {
            return top.HasTile && !top.BottomSlope && top.TileType >= 0 && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType];
        }

        public virtual bool CanAnchorBottom(int i, int j, Tile tile, Tile bottom) {
            return bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && bottom.TileType >= 0 && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]);
        }

        public virtual bool CanAnchorLeft(int i, int j, Tile tile, Tile left) {
            return left.HasTile && left.TileType >= 0 && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType];
        }

        public virtual bool CanAnchorRight(int i, int j, Tile tile, Tile right) {
            return right.HasTile && right.TileType >= 0 && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType];
        }

        public virtual bool CanAnchor(int i, int j, Tile tile, Tile anchorTile) {
            return true;
        }

        public void GemFrame(int i, int j) {
            var tile = Framing.GetTileSafely(i, j);
            var anchorTile = Framing.GetTileSafely(i, j - 1);
            tile.TileFrameX = 0;
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorTop(i, j, tile, anchorTile)) {
                if (tile.TileFrameY < 0 || tile.TileFrameY > 36) {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);
                }
                return;
            }

            anchorTile = Framing.GetTileSafely(i, j + 1);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorBottom(i, j, tile, anchorTile)) {
                if (tile.TileFrameY < 54 || tile.TileFrameY > 90) {
                    tile.TileFrameY = (short)(54 + WorldGen.genRand.Next(3) * 18);
                }
                return;
            }

            anchorTile = Framing.GetTileSafely(i - 1, j);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorLeft(i, j, tile, anchorTile)) {
                if (tile.TileFrameY < 108 || tile.TileFrameY > 144) {
                    tile.TileFrameY = (short)(108 + WorldGen.genRand.Next(3) * 18);
                }
                return;
            }

            anchorTile = Framing.GetTileSafely(i + 1, j);
            if (CanAnchor(i, j, tile, anchorTile) && CanAnchorRight(i, j, tile, anchorTile)) {
                if (tile.TileFrameY < 162 || tile.TileFrameY > 198) {
                    tile.TileFrameY = (short)(162 + WorldGen.genRand.Next(3) * 18);
                }
                return;
            }

            WorldGen.KillTile(i, j);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            int item = TileLoader.GetItemDropFromTypeAndStyle(Type, Main.tile[i, j].TileFrameX / 18);
            if (item > 0) {
                return new Item[1]
                {
                    new Item(item)
                };
            }

            return null;
        }

        public TileObjectData GetObjectData(int i, int j) {
            var tile = Main.tile[i, j];
            int style = tile.TileFrameY / 18;
            int alt = tile.TileFrameY / 54;
            var objectData = TileObjectData.GetTileData(tile.TileType, style, alt);
            return objectData;
        }
    }
}