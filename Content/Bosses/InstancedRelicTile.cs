using Aequus.Common.Tiles;
using Aequus.Core.Graphics.Tiles;
using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Bosses;

internal class InstancedRelicTile : InstancedModTile, ISpecialTileRenderer {
    private readonly IRelicRenderer _renderer;

    public InstancedRelicTile(String name, IRelicRenderer renderer) : base($"{name}Relic", $"{typeof(InstancedTrophyTile).NamespaceFilePath()}/Trophies/{name}Relic") {
        _renderer = renderer;
    }

    public override String Texture => AequusTextures.Tile(TileID.MasterTrophyBase);

    private const Int32 FrameWidth = 18 * 3;
    private const Int32 FrameHeight = 18 * 4;

    public override void SetStaticDefaults() {
        Main.tileShine[Type] = 400;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleHorizontal = false;
        TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.styleLineSkipVisualOverride = 0;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        AdjTiles = new Int32[] { TileID.MasterTrophyBase, };

        AddMapEntry(new Color(233, 207, 94, 255), Language.GetText("MapObject.Relic"));
    }

    public override Boolean CreateDust(Int32 i, Int32 j, ref Int32 type) {
        return false;
    }

    public override void SetDrawPositions(Int32 i, Int32 j, ref Int32 width, ref Int32 offsetY, ref Int32 height, ref Int16 tileFrameX, ref Int16 tileFrameY) {
        tileFrameX %= FrameWidth;
        tileFrameY %= FrameHeight * 2;
    }

    public override void DrawEffects(Int32 i, Int32 j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
        }
    }

    void ISpecialTileRenderer.Render(Int32 i, Int32 j, Byte layer) {
        var p = new Point(i, j);
        var tile = Main.tile[p];
        if (!tile.HasTile) {
            return;
        }

        var relicFrame = new Rectangle(tile.TileFrameX, FrameHeight * 4, FrameWidth, FrameHeight);
        var origin = relicFrame.Size() / 2f;
        var worldCoordinates = p.ToWorldCoordinates(24f, 64f);
        var relicColor = Lighting.GetColor(p.X, p.Y);
        var relicEffects = tile.TileFrameY / FrameHeight != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        Single offset = (Single)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
        var drawCoordinates = worldCoordinates - Main.screenPosition + new Vector2(0f, -30f) + new Vector2(0f, offset * 4f);

        _renderer.DrawRelic(i, j, drawCoordinates, relicColor, relicEffects, offset);
    }
}
