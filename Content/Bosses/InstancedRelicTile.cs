using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Bosses;

internal class InstancedRelicTile : InstancedModTile, ISpecialTileRenderer {
    private readonly IRelicRenderer _renderer;

    public InstancedRelicTile(string name, IRelicRenderer renderer) : base($"{name}Relic", $"{typeof(InstancedTrophyTile).NamespaceFilePath()}/Trophies/{name}Relic") {
        _renderer = renderer;
    }

    public override string Texture => AequusTextures.Tile(TileID.MasterTrophyBase);

    private const int FrameWidth = 18 * 3;
    private const int FrameHeight = 18 * 4;

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

        AdjTiles = new int[] { TileID.MasterTrophyBase, };

        AddMapEntry(new Color(233, 207, 94, 255), Language.GetText("MapObject.Relic"));
    }

    public override bool CreateDust(int i, int j, ref int type) {
        return false;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        tileFrameX %= FrameWidth;
        tileFrameY %= FrameHeight * 2;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
        }
    }

    void ISpecialTileRenderer.Render(int i, int j, byte layer) {
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

        float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
        var drawCoordinates = worldCoordinates - Main.screenPosition + new Vector2(0f, -30f) + new Vector2(0f, offset * 4f);

        _renderer.DrawRelic(i, j, drawCoordinates, relicColor, relicEffects, offset);
    }
}
