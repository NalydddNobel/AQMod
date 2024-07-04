using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Entities.Tiles.Components;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace AequusRemake.Content.Tiles.Furniture.Trash;

public class TrashChest : UnifiedModChest, IPlaceChestHook {
    public override bool LoadTrappedChest => false;

    public override string LocalizationCategory => "Tiles.Furniture";

    public override void SafeSetStaticDefaults() {
        TileObjectData.newTile.Width = 3;
        TileObjectData.newTile.Origin = new(1, 1);
        AddMapEntry(Color.Green, CreateMapEntryName(), MapChestName);
    }

    protected override int FindEmptyChest(int x, int y, int type, int style, int direction, int alternate) {
        int chestId = -1;
        int chestCount = 0;
        for (int i = 0; i < Main.maxChests; i++) {
            var chest = Main.chest[i];
            if (chest != null) {
                if (chest.x == x && chest.y == y) {
                    return -1;
                }
            }
            else {
                chestCount++;
                if (chestId == -1) {
                    chestId = i;
                }
            }
        }

        return chestCount > 1 ? chestId : -1;
    }

    protected override int AfterPlacement_Hook(int x, int y, int type, int style, int direction, int alternate) {
        var baseCoords = new Point16(x, y);
        TileObjectData.OriginToTopLeft(type, style, ref baseCoords);
        for (int i = 0; i < 2; i++) {
            int chestId = Chest.FindEmptyChest(baseCoords.X + i, baseCoords.Y, type, style, direction, alternate);
            if (chestId == -1) {
                continue;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Chest.CreateChest(baseCoords.X + i, baseCoords.Y, chestId);
            }
            else {
                SendChestUpdate(x, y, style);
                break;
            }
        }

        return -1;
    }

    protected override void GetChestLocation(int i, int j, in Tile tileCache, out int chestX, out int chestY) {
        base.GetChestLocation(i, j, tileCache, out chestX, out chestY);
        if (Main.tile[i, j].TileFrameX > 18) {
            chestX++;
        }
    }

    protected override void GetChestHoverLocation(int i, int j, double mouseX, double mouseY, in Tile tileCache, out int chestX, out int chestY) {
        base.GetChestHoverLocation(i, j, mouseX, mouseY, tileCache, out chestX, out chestY);

        switch (Main.tile[i, j].TileFrameX / 18 % FrameWidth) {
            case 1:
                if (mouseX > 0.5) {
                    chestX++;
                }
                break;
        }
    }

    public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY) {
        width = 2;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        int frameX = Main.tile[i, j].TileFrameX % FrameWidth;
        int frameY = Main.tile[i, j].TileFrameY % FrameHeight;
        if (frameX != 0 || frameY != 0) {
            return false;
        }

        var data = TileObjectData.GetTileData(Main.tile[i, j]);
        var drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;


        DrawDumpster(TextureAssets.Tile[Type].Value, c => c, c => c);
        bool selectedLeft = Main.InSmartCursorHighlightArea(i, j, out var actuallySelectedLeft);
        bool selectedRight = Main.InSmartCursorHighlightArea(i + 2, j, out var actuallySelectedRight);
        if (selectedLeft || selectedRight) {
            DrawDumpster(TextureAssets.HighlightMask[Type].Value, c => Colors.GetSelectionGlowColor(actuallySelectedLeft, (c.R + c.G + c.B) / 3), c => Colors.GetSelectionGlowColor(actuallySelectedRight, (c.R + c.G + c.B) / 3));
        }

        void DrawDumpster(Texture2D texture, Func<Color, Color> leftSideColor, Func<Color, Color> rightSideColor) {
            int chestLeft = Chest.FindChest(i, j);
            int frameLeft = chestLeft != -1 ? Main.chest[chestLeft].frame : 0;
            int chestRight = Chest.FindChest(i + 1, j);
            int frameRight = chestRight != -1 ? Main.chest[chestRight].frame : 0;
            for (int l = 0; l < 2; l++) {
                var lightColor = Lighting.GetColor(i, j + l);
                spriteBatch.Draw(texture, drawCoordinates + new Vector2(0f, l * 16f), new(0, 18 * l + FrameHeight * frameLeft, 16, data.CoordinateHeights[l]), leftSideColor(lightColor));
            }
            for (int l = 0; l < 2; l++) {
                var lightColor = Lighting.GetColor(i + 1, j + l);
                spriteBatch.Draw(texture, drawCoordinates + new Vector2(16f, l * 16f), new(18, 18 * l + FrameHeight * frameLeft, 8, data.CoordinateHeights[l]), leftSideColor(lightColor));
                spriteBatch.Draw(texture, drawCoordinates + new Vector2(24f, l * 16f), new(26, 18 * l + FrameHeight * frameRight, 8, data.CoordinateHeights[l]), rightSideColor(lightColor));
            }
            for (int l = 0; l < 2; l++) {
                var lightColor = Lighting.GetColor(i + 2, j + l);
                spriteBatch.Draw(texture, drawCoordinates + new Vector2(32f, l * 16f), new(36, 18 * l + FrameHeight * frameRight, 16, data.CoordinateHeights[l]), rightSideColor(lightColor));
            }
        }

        return false;
    }

    int IPlaceChestHook.PlaceChest(int x, int y, int style, bool notNearOtherChests) {
        // Cannot place on boulders
        for (int i = 0; i < 3; i++) {
            if (TileID.Sets.Boulders[Main.tile[x + i, y + 1].TileType]) {
                return -1;
            }
        }

        if (!TileObject.CanPlace(x, y, Type, style, 1, out var objectData) || (notNearOtherChests && Chest.NearOtherChests(x - 1, y - 1))) {
            return -1;
        }

        ((IPlaceChestHook)this).PlaceChestDirect(x, y, style, -1);
        int chestId = -1;
        for (int i = 0; i < 2; i++) {
            int nextChestId = Chest.CreateChest(objectData.xCoord + i, objectData.yCoord);
            if (nextChestId != -1) {
                chestId = nextChestId;
            }
        }
        if (chestId != 1 && Main.netMode == NetmodeID.MultiplayerClient) {
            SendChestUpdate(x, y, style);
        }
        return chestId;
    }

    bool IPlaceChestHook.PlaceChestDirect(int x, int y, int style, int id) {
        // Adjust tile data
        for (int i = 0; i < 3; i++) {
            short frameX = (short)(18 * i + style * FrameWidth);
            for (int j = 0; j < 2; j++) {
                var tile = Main.tile[x + i, y + j];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileType = Type;
                tile.TileFrameX = frameX;
                tile.TileFrameY = (short)(18 * j);
            }
        }

        // Create chests
        for (int i = 0; i < 2; i++) {
            Chest.CreateChest(x + i, y, id);
        }
        return false;
    }

    public override bool CanKillTile(int i, int j, ref bool blockDamaged) {
        int left = i - Main.tile[i, j].TileFrameX % FrameWidth / 18;
        int top = j - Main.tile[i, j].TileFrameY % FrameHeight / 18;
        for (int k = left; k < left + 3; k++) {
            if (!Chest.CanDestroyChest(k, top)) {
                return false;
            }
        }
        return true;
    }

    public override bool CanReplace(int i, int j, int tileTypeBeingPlaced) {
        return false;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        return !WorldGen.destroyObject;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        for (int k = 0; k < 3; k++) {
            Chest.DestroyChest(i + k, j);
        }
    }

    public bool GetChestIds(int i, int j, out int leftChestId, out int rightChestId) {
        leftChestId = -1;
        rightChestId = -1;
        if (Main.tile[i, j].TileType != Type) {
            return false;
        }

        int left = i - Main.tile[i, j].TileFrameX % FrameWidth / 18;
        int top = j - Main.tile[i, j].TileFrameY % FrameHeight / 18;

        leftChestId = Chest.FindChest(left, top);
        rightChestId = Chest.FindChest(left + 1, top);
        return leftChestId != -1 && rightChestId != -1;
    }
}
