using Aequus.Common.Tiles;
using Aequus.Core.Graphics.Tiles;
using System;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Tiles.Furniture.Oblivion;

public class OblivionCraftingStation : ModTile, ISpecialTileRenderer {
    public static ModItem Item { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this);
        Mod.AddContent(Item);
    }

    public override void Unload() {
        Item = null;
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.CoordinateHeights = new Int32[] { 16, 16, 18 };
        TileObjectData.addTile(Type);
        DustType = DustID.Ash;
        AdjTiles = new Int32[] { TileID.DemonAltar };
        MinPick = 110;
        AddMapEntry(new Color(100, 40, 50), Item.DisplayName);
    }

    public override void DrawEffects(Int32 i, Int32 j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileCache.TileFrameX % 48 == 0 && drawData.tileCache.TileFrameY % 48 == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
        }
    }

    public override void SetDrawPositions(Int32 i, Int32 j, ref Int32 width, ref Int32 offsetY, ref Int32 height, ref Int16 tileFrameX, ref Int16 tileFrameY) {
        tileFrameX += 54;
    }

    public void Render(Int32 i, Int32 j, Byte layer) {
        Vector2 portalPosition = new Vector2(i * 16f + 24f, j * 16f + 16f) - Main.screenPosition;
        Single portalAnimation = Main.GlobalTimeWrappedHourly * 5f;
        Single portalSquishMagnitude = 0.1f;
        Vector2 portalHoveringPosition = portalPosition + new Vector2(0f, Helper.Oscillate(Main.GlobalTimeWrappedHourly, -2f, 2f));

        Texture2D portalTexture = AequusTextures.OblivionCraftingStation_Portal.Value;
        Main.spriteBatch.Draw(portalTexture, portalHoveringPosition, null, Color.White, 0f, portalTexture.Size() / 2f, new Vector2(1f + MathF.Sin(portalAnimation) * portalSquishMagnitude, 1f + MathF.Cos(portalAnimation) * portalSquishMagnitude), SpriteEffects.None, 0f);

        Texture2D stickTexture = AequusTextures.OblivionCraftingStation_Stick.Value;
        Rectangle stickFrame = stickTexture.Frame(verticalFrames: 10, frameY: (Int32)Main.GameUpdateCount / 6 % 10);
        stickFrame.Height -= 2;
        Vector2 stickOrigin = stickFrame.Size() / 2f;
        Color stickColor = Lighting.GetColor(i + 1, j + 1);
        Vector2 stickPositionOffset = new Vector2(17f, 1f);
        Main.spriteBatch.Draw(stickTexture, portalPosition + stickPositionOffset, stickFrame, stickColor, 0f, stickOrigin, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);
        Main.spriteBatch.Draw(stickTexture, portalPosition - stickPositionOffset, stickFrame, stickColor, 0f, stickOrigin, 1f, SpriteEffects.None, 0f);
    }
}