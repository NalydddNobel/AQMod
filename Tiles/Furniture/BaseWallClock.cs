using Aequus.Common.Drawing;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture;
public abstract class BaseWallClock : ModTile, ITileDrawSystem {
    int ITileDrawSystem.Type => Type;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.Clock[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
        TileObjectData.newTile.Width = 2;
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;
        TileObjectData.addTile(Type);
        DustType = 7;
        AddMapEntry(new Color(120, 85, 60), TextHelper.GetText("MapObject.WallClocks"));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override bool RightClick(int i, int j) {
        Main.NewText($"Time: {TextHelper.WatchTime(Main.time, Main.dayTime)}", new Color(255, 240, 20));
        return true;
    }

    public virtual void DrawClockHands(int i, int j, SpriteBatch spriteBatch) {
        var p = new Point(i, j);
        var texture = TextureAssets.Tile[Type].Value;
        var handFrame = new Rectangle(Main.tile[p].TileFrameY - 18, 36, 16, 16);
        var origin = new Vector2(7f, 8f);
        var drawCoordinates = new Vector2(p.X * 16f - 1f, p.Y * 16f + 2f) - Main.screenPosition;
        var color = Lighting.GetColor(p);
        float progress = (float)Utils.GetDayTimeAs24FloatStartingFromMidnight() / 24f;
        handFrame.Y += 16;
        spriteBatch.Draw(texture, drawCoordinates, handFrame, color, progress * MathHelper.TwoPi * 48f + MathHelper.Pi, origin, 1f, SpriteEffects.None, 0f);
        handFrame.Y -= 16;
        spriteBatch.Draw(texture, drawCoordinates, handFrame, color, progress * MathHelper.TwoPi * 2f + MathHelper.Pi, origin, 1f, SpriteEffects.None, 0f);
    }

    void DrawAllClocks(SpriteBatch sb) {
        foreach (Point p in this.GetDrawPoints()) {
            DrawClockHands(p.X, p.Y, sb);
        }
    }

    bool IGridDrawSystem.Accept(Point p) {
        return Main.tile[p].TileFrameY == 18 && Main.tile[p].TileFrameX % 36 == 18;
    }

    void IDrawSystem.Activate() {
        DrawLayers.Instance.PostDrawMasterRelics += DrawAllClocks;
    }

    void IDrawSystem.Deactivate() {
        DrawLayers.Instance.PostDrawMasterRelics -= DrawAllClocks;
    }
}