using Aequus.Core.ContentGeneration;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Furniture.Trash;

internal class TrashClock(UnifiedFurniture parent) : InstancedFurnitureClock(parent) {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.Clock[Type] = true;

        DustType = Parent.DustType;
        AdjTiles = [TileID.GrandfatherClocks];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 3, 0);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        AddMapEntry(CommonColor.MapWoodFurniture, Lang.GetItemName(ItemID.GrandfatherClock));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        Texture2D texture = GetTileTexture(i, j, tile);
        Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset;
        Color lightColor = GetTileColor(i, j, tile);

        if (TileDrawing.IsVisible(tile)) {
            Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, tile.TileFrameY <= 0 ? 16 : 18);
            spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        // Clock Digits inherit the paint color and light of the bottom right tile.
        if (tile.TileFrameX == 36 && tile.TileFrameY == 18) {
            DrawDigits(spriteBatch, texture, drawCoordinates, lightColor.SaturationMultiply(0.4f));
        }

        return false;
    }

    private void DrawDigits(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawCoordinates, Color lightColor) {
        int[] digits = Helper.GetTimeDigits(Helper.Get24HourTimeStartingAtMidnight());

        for (int k = 0; k < digits.Length; k++) {
            int digit = digits[k] - 1;
            Rectangle digitFrame = digit switch {
                -1 => new Rectangle(8, 62, 6, 10),
                _ => new Rectangle(8 * (digit % 4), 38 + 12 * (digit / 4), 6, 10),
            };
            //Rectangle digitFrame = new Rectangle(0, 38, 6, 10);
            int offset = 8 * k;
            offset += k > 1 ? 4 : 0;
            spriteBatch.Draw(texture, drawCoordinates + new Vector2(-26f + offset, 2f), digitFrame, lightColor * 2f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        spriteBatch.Draw(texture, drawCoordinates + new Vector2(-12f, 2f), new Rectangle(16, 62, 6, 10), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }
}