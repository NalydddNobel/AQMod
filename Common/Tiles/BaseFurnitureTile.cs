using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Common.Tiles;

public class BaseFurnitureTile {
    /// <summary>
    /// A base furntiure tile, Furniture in Aequus is handled on an individual ModTile basis instead of through a big multi-styled tile.
    /// </summary>
    public abstract class Furniture : ModTile {
        public abstract System.Int32 FurnitureDust { get; }
        public virtual System.Boolean DieInLava => true;
        public abstract Color MapColor { get; }
        private System.Int32 _itemDropCache;

        public System.Int32 ItemDrop {
            get {
                if (_itemDropCache == 0) {
                    _itemDropCache = TileLoader.GetItemDropFromTypeAndStyle(Type, 0);
                }
                return _itemDropCache;
            }
        }

        protected virtual void FurnitureDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = DieInLava;
            DustType = FurnitureDust;
        }
    }
    public abstract class LightedFurniture : Furniture {
        public virtual System.Boolean DieInWater => false;
        public abstract Vector3 LightColor { get; }

        protected override void FurnitureDefaults() {
            base.FurnitureDefaults();
            Main.tileWaterDeath[Type] = DieInWater;
            Main.tileLighted[Type] = true;
        }
    }

    public abstract class Bathtub : Furniture {
        public override Color MapColor => new Color(144, 148, 144);

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AddMapEntry(MapColor, Lang.GetItemName(ItemID.Bathtub));
            TileID.Sets.DisableSmartCursor[Type] = true;
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }
    }

    public abstract class Bed : Furniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE;

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true;
            TileID.Sets.IsValidSpawnPoint[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new System.Int32[] { TileID.Beds };
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);
            AddMapEntry(MapColor, Lang.GetItemName(ItemID.Bed));
        }

        public override System.Boolean HasSmartInteract(System.Int32 i, System.Int32 j, SmartInteractScanSettings settings) {
            return true;
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }

        public override System.Boolean RightClick(System.Int32 i, System.Int32 j) {
            var player = Main.LocalPlayer;
            var tile = Main.tile[i, j];
            System.Int32 spawnX = i - tile.TileFrameX / 18 + (tile.TileFrameX >= 72 ? 5 : 2);
            System.Int32 spawnY = j + 2;
            if (tile.TileFrameY % 38 != 0) {
                spawnY--;
            }

            if (!Player.IsHoveringOverABottomSideOfABed(i, j)) {
                if (player.IsWithinSnappngRangeToTile(i, j, 96)) {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, i, j);
                }
            }
            else {
                player.FindSpawn();
                if (player.SpawnX == spawnX && player.SpawnY == spawnY) {
                    player.RemoveSpawn();
                    Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), System.Byte.MaxValue, 240, 20);
                }
                else if (Player.CheckSpawn(spawnX, spawnY)) {
                    player.ChangeSpawn(spawnX, spawnY);
                    Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), System.Byte.MaxValue, 240, 20);
                }
            }

            return true;
        }

        public override void MouseOver(System.Int32 i, System.Int32 j) {
            Player player = Main.LocalPlayer;

            if (!Player.IsHoveringOverABottomSideOfABed(i, j)) {
                if (player.IsWithinSnappngRangeToTile(i, j, 96)) {
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ItemID.SleepingIcon;
                }
            }
            else {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ItemDrop;
            }
        }
    }

    public abstract class Bookcase : Furniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE;

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            AddMapEntry(MapColor, Lang.GetItemName(ItemID.Bookcase));

            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new System.Int32[] { TileID.Bookcases };
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }
    }

    public abstract class Candelabra : LightedFurniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE_LIGHTED;

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            Main.tileNoAttach[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            if (DieInWater)
                TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            if (DieInLava)
                TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            AddMapEntry(MapColor, Lang.GetItemName(ItemID.Candelabra));
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }

        public override void HitWire(System.Int32 i, System.Int32 j) {
            Tile tile = Main.tile[i, j];
            System.Int32 topX = i - tile.TileFrameX / 18 % 2;
            System.Int32 topY = j - tile.TileFrameY / 18 % 2;
            System.Int16 frameAdjustment = (System.Int16)(tile.TileFrameX >= 36 ? -36 : 36);
            Main.tile[topX, topY].TileFrameX += frameAdjustment;
            Main.tile[topX, topY + 1].TileFrameX += frameAdjustment;
            Main.tile[topX + 1, topY].TileFrameX += frameAdjustment;
            Main.tile[topX + 1, topY + 1].TileFrameX += frameAdjustment;
            Wiring.SkipWire(topX, topY);
            Wiring.SkipWire(topX, topY + 1);
            Wiring.SkipWire(topX + 1, topY);
            Wiring.SkipWire(topX + 1, topY + 1);
            NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
        }

        public override void ModifyLight(System.Int32 i, System.Int32 j, ref System.Single r, ref System.Single g, ref System.Single b) {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX < 36) {
                r = LightColor.X;
                g = LightColor.Y;
                b = LightColor.Z;
            }
        }
    }

    public abstract class Candle : LightedFurniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE_LIGHTED;

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            Main.tileNoAttach[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            if (DieInWater)
                TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            if (DieInLava)
                TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.Table, TileObjectData.newTile.Width, 0);

            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            AddMapEntry(MapColor, Lang.GetItemName(ItemID.Candle));
        }

        public override void MouseOver(System.Int32 i, System.Int32 j) {
            var player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemDrop;
        }

        public override System.Boolean RightClick(System.Int32 i, System.Int32 j) {
            WorldGen.KillTile(i, j);
            return true;
        }

        public override void HitWire(System.Int32 i, System.Int32 j) {
            Tile tile = Main.tile[i, j];
            System.Int32 topX = i - tile.TileFrameX / 18 % 1;
            System.Int32 topY = j - tile.TileFrameY / 18 % 1;
            System.Int16 frameAdjustment = (System.Int16)(tile.TileFrameX >= 18 ? -18 : 18);
            Main.tile[topX, topY].TileFrameX += frameAdjustment;
            Wiring.SkipWire(topX, topY);
            NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
        }

        public override void ModifyLight(System.Int32 i, System.Int32 j, ref System.Single r, ref System.Single g, ref System.Single b) {
            if (Main.tile[i, j].TileFrameX < 18) {
                r = LightColor.X;
                g = LightColor.Y;
                b = LightColor.Z;
            }
        }
    }

    public abstract class Chandelier : LightedFurniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE_LIGHTED;

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            if (DieInWater) {
                TileObjectData.newTile.WaterDeath = true;
                TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            }
            if (DieInLava)
                TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            AddMapEntry(MapColor, Language.GetText("MapObject.Chandelier"));
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }

        public override void HitWire(System.Int32 i, System.Int32 j) {
            Tile tile = Main.tile[i, j];
            System.Int32 topX = i - tile.TileFrameX / 18 % 3;
            System.Int32 topY = j - tile.TileFrameY / 18 % 3;
            System.Int16 frameAdjustment = (System.Int16)(tile.TileFrameX >= 54 ? -54 : 54);
            Main.tile[topX, topY].TileFrameX += frameAdjustment;
            Main.tile[topX, topY + 1].TileFrameX += frameAdjustment;
            Main.tile[topX, topY + 2].TileFrameX += frameAdjustment;
            Main.tile[topX + 1, topY].TileFrameX += frameAdjustment;
            Main.tile[topX + 1, topY + 1].TileFrameX += frameAdjustment;
            Main.tile[topX + 1, topY + 2].TileFrameX += frameAdjustment;
            Main.tile[topX + 2, topY].TileFrameX += frameAdjustment;
            Main.tile[topX + 2, topY + 1].TileFrameX += frameAdjustment;
            Main.tile[topX + 2, topY + 2].TileFrameX += frameAdjustment;
            Wiring.SkipWire(topX, topY);
            Wiring.SkipWire(topX, topY + 1);
            Wiring.SkipWire(topX, topY + 2);
            Wiring.SkipWire(topX + 1, topY);
            Wiring.SkipWire(topX + 1, topY + 1);
            Wiring.SkipWire(topX + 1, topY + 2);
            Wiring.SkipWire(topX + 2, topY);
            Wiring.SkipWire(topX + 2, topY + 1);
            Wiring.SkipWire(topX + 2, topY + 2);
            NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
        }

        public override void ModifyLight(System.Int32 i, System.Int32 j, ref System.Single r, ref System.Single g, ref System.Single b) {
            if (Main.tile[i, j].TileFrameX < 54) {
                r = LightColor.X;
                g = LightColor.Y;
                b = LightColor.Z;
            }
        }
    }

    public abstract class Chair : Furniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE;

        protected virtual void AddMapEntry() {
            AddMapEntry(MapColor, Language.GetText("MapObject.Chair"));
        }

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            Main.tileNoAttach[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            AddMapEntry();

            AdjTiles = new System.Int32[] { TileID.Chairs };
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }

        public override System.Boolean HasSmartInteract(System.Int32 i, System.Int32 j, SmartInteractScanSettings settings) {
            return true;
        }

        public override void MouseOver(System.Int32 i, System.Int32 j) {
            Player player = Main.LocalPlayer;
            if (player.IsWithinSnappngRangeToTile(i, j, 40)) {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ItemDrop;
                player.cursorItemIconReversed = Main.tile[i, j].TileFrameX == 0;
            }
        }

        public override System.Boolean RightClick(System.Int32 i, System.Int32 j) {
            Player player = Main.LocalPlayer;
            if (player.IsWithinSnappngRangeToTile(i, j, 40)) {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
                return true;
            }
            return false;
        }
    }

    public abstract class Toilet : Chair {
        protected override void AddMapEntry() {
            AddMapEntry(MapColor, Language.GetText("MapObject.Toilet"));
        }

        public override void HitWire(System.Int32 i, System.Int32 j) {
            Tile tile = Main.tile[i, j];

            System.Int32 spawnX = i;
            System.Int32 spawnY = j - tile.TileFrameY % 40 / 18;

            Wiring.SkipWire(spawnX, spawnY);
            Wiring.SkipWire(spawnX, spawnY + 1);

            if (Wiring.CheckMech(spawnX, spawnY, 60)) {
                Projectile.NewProjectile(Wiring.GetProjectileSource(spawnX, spawnY), spawnX * 16 + 8, spawnY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
            }
        }
    }

    public abstract class Clock : Furniture {
        public override Color MapColor => CommonColor.TILE_FURNITURE;

        public override void SetStaticDefaults() {
            FurnitureDefaults();
            Main.tileNoAttach[Type] = true;
            TileID.Sets.Clock[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16 };
            TileObjectData.addTile(Type);

            AddMapEntry(MapColor, Lang.GetItemName(ItemID.GrandfatherClock));
            AdjTiles = new System.Int32[] { TileID.GrandfatherClocks };

            TileID.Sets.DisableSmartCursor[Type] = true;
        }

        public override void NumDust(System.Int32 i, System.Int32 j, System.Boolean fail, ref System.Int32 num) {
            num = fail ? 1 : 3;
        }

        public override System.Boolean RightClick(System.Int32 x, System.Int32 y) {
            Main.NewText($"Time: {ExtendLanguage.WatchTime(Main.time, Main.dayTime)}", new Color(255, 240, 20));
            return true;
        }
    }

    // ZZZZzzzzZZ I'll do the rest later
}