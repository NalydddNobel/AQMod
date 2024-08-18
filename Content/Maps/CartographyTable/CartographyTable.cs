using Aequus.Common.ContentTemplates.Generic;
using Aequus.Content.Biomes.Meadows.Tiles;
using System;
using System.Diagnostics;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Content.Maps.CartographyTable;

public class CartographyTable : ModTile, IAddRecipes {
    public static CartographyTable Instance => ModContent.GetInstance<CartographyTable>();

    private readonly ModItem Item;
    public CartographyTable() {
        Item = new InstancedTileItem(this);
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.Origin = new(1, 3);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.addTile(Type);
        AddMapEntry(Color.SaddleBrown);
        AddMapEntry(new Color(15, 29, 42)); // Empty
        AddMapEntry(new Color(35, 62, 97)); // Wall
        AddMapEntry(new Color(39, 71, 145)); // BG Object
        AddMapEntry(new Color(69, 95, 186)); // Block
        HitSound = SoundID.Dig;
        DustType = DustID.WoodFurniture;
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    void IAddRecipes.AddRecipes(Aequus aequus) {
        Item.CreateRecipe()
            .AddIngredient(ModContent.GetInstance<MeadowWood>().Item, 25)
            .AddTile(TileID.Sawmill)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.GlassKiln);
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        int offset = Math.Clamp(WorldGen.GetWorldSize(), 0, 2);
        drawData.addFrX = offset * 72;
    }

    public override bool RightClick(int i, int j) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModContent.GetInstance<ServerMapRequestPacket>().SendReset(Main.myPlayer);
        }
        return true;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX == 0 && tile.TileFrameY == 0) {
            DrawDownloadBar(i, j, spriteBatch);
        }
        return true;
    }

    #region Map Download Percentage Display
    public float DownloadProgress { get; private set; }
    public float UploadProgress { get; private set; }

    private readonly Stopwatch _netStopwatch = new();

    public void SetDownloadProgress(float value) {
        _netStopwatch.Restart();
        DownloadProgress = value;
    }

    public void SetUploadProgress(float value) {
        _netStopwatch.Restart();
        UploadProgress = value;
    }

    public void EndNet() {
        DownloadProgress = 0f;
        UploadProgress = 0f;
        _netStopwatch.Stop();
    }

    private void DrawDownloadBar(int i, int j, SpriteBatch sb) {
        if (_netStopwatch.ElapsedMilliseconds > 10000) {
            EndNet();
        }

        Vector2 drawCoords = new Vector2(i + 1.5f, j - 1f).ToWorldCoordinates() - Main.screenPosition + Helper.TileDrawOffset;


        float topBarProgress = DownloadProgress;
        float bottomBarProgress = UploadProgress;

        if (topBarProgress <= 0f && bottomBarProgress <= 0f) {
            return;
        }

        Texture2D bar = AequusTextures.CartographyTableDownloadBar.Value;
        Color color = Color.White;
        Rectangle barFrame = new Rectangle(32, 2, 62, 14);
        Vector2 barFillCoords = drawCoords - new Vector2(barFrame.Width / 2f - 4, 1f);
        Vector2 arrowCoords = drawCoords + new Vector2(barFrame.Width / 2f + 4, 1f);
        float barFillScale = barFrame.Width - 8f;

        // The bar.
        sb.Draw(bar, drawCoords, barFrame, color, 0f, barFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);

        // Throbber for decor.
        int throbberFrameTime = (int)Math.Clamp(Main.GlobalTimeWrappedHourly * 12f % 8f, 0f, 8f);
        Rectangle throbberFrame = new Rectangle(0, 18 * throbberFrameTime, 18, 18);
        sb.Draw(bar, drawCoords - new Vector2(barFrame.Width / 2f + 18f, 0f), throbberFrame, color, 0f, throbberFrame.Size() / 2f, 1f, SpriteEffects.None, 0f);

        // Draw download sprites.
        if (topBarProgress > 0f) {
            Rectangle dlFrame = new Rectangle(32, 18, 4, 2);
            sb.Draw(bar, barFillCoords + new Vector2(0f, -2f), dlFrame, color, 0f, Vector2.Zero, new Vector2(1f / dlFrame.Width * topBarProgress * barFillScale, 1f), SpriteEffects.None, 0f);

            // Arrow indicating that we are downloading map data.
            if (topBarProgress < 1f) {
                Rectangle dlArrow = new Rectangle(38, 18, 14, 16);
                sb.Draw(bar, arrowCoords, dlArrow, color, 0f, new Vector2(0f, dlArrow.Height / 2f), 1f, SpriteEffects.None, 0f);
                arrowCoords.X += 18f;
            }
        }

        // Draw upload sprites.
        if (bottomBarProgress > 0f) {
            Rectangle ulFrame = new Rectangle(32, 36, 4, 2);
            sb.Draw(bar, barFillCoords + new Vector2(0f, 2f), ulFrame, color, 0f, Vector2.Zero, new Vector2(1f / ulFrame.Width * bottomBarProgress * barFillScale, 1f), SpriteEffects.None, 0f);

            // Arrow indicating that we are uploading map data.
            if (bottomBarProgress < 1f) {
                Rectangle ulArrow = new Rectangle(38, 36, 14, 16);
                sb.Draw(bar, arrowCoords, ulArrow, color, 0f, new Vector2(0f, ulArrow.Height / 2f), 1f, SpriteEffects.None, 0f);
                arrowCoords.X += 18f;
            }
        }
    }
    #endregion
}
