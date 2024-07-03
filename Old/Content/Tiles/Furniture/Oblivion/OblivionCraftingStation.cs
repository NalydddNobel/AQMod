using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Graphics.Tiles;
using System;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Aequu2.Old.Content.Tiles.Furniture.Oblivion;

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
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
        TileObjectData.addTile(Type);
        DustType = DustID.Ash;
        AdjTiles = new int[] { TileID.DemonAltar };
        MinPick = 110;
        AddMapEntry(new Color(100, 40, 50), Item.DisplayName);
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileCache.TileFrameX % 48 == 0 && drawData.tileCache.TileFrameY % 48 == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
        }
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        tileFrameX += 54;
    }

    public void Render(int i, int j, byte layer) {
        Vector2 portalPosition = new Vector2(i * 16f + 24f, j * 16f + 16f) - Main.screenPosition;
        float portalAnimation = Main.GlobalTimeWrappedHourly * 5f;
        float portalSquishMagnitude = 0.1f;
        Vector2 portalHoveringPosition = portalPosition + new Vector2(0f, Helper.Oscillate(Main.GlobalTimeWrappedHourly, -2f, 2f));

        Texture2D portalTexture = Aequu2Textures.OblivionCraftingStation_Portal.Value;
        Main.spriteBatch.Draw(portalTexture, portalHoveringPosition, null, Color.White, 0f, portalTexture.Size() / 2f, new Vector2(1f + MathF.Sin(portalAnimation) * portalSquishMagnitude, 1f + MathF.Cos(portalAnimation) * portalSquishMagnitude), SpriteEffects.None, 0f);

        Texture2D stickTexture = Aequu2Textures.OblivionCraftingStation_Stick.Value;
        Rectangle stickFrame = stickTexture.Frame(verticalFrames: 10, frameY: (int)Main.GameUpdateCount / 6 % 10);
        stickFrame.Height -= 2;
        Vector2 stickOrigin = stickFrame.Size() / 2f;
        Color stickColor = Lighting.GetColor(i + 1, j + 1);
        Vector2 stickPositionOffset = new Vector2(17f, 1f);
        Main.spriteBatch.Draw(stickTexture, portalPosition + stickPositionOffset, stickFrame, stickColor, 0f, stickOrigin, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);
        Main.spriteBatch.Draw(stickTexture, portalPosition - stickPositionOffset, stickFrame, stickColor, 0f, stickOrigin, 1f, SpriteEffects.None, 0f);
    }
}