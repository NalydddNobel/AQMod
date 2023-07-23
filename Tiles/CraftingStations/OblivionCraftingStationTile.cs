using Aequus.Common.Rendering.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.CraftingStations {
    public class OblivionCraftingStationTile : ModTile, ISpecialTileRenderer {
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
            AddMapEntry(new(100, 40, 50), TextHelper.GetItemName<OblivionCraftingStationItem>());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
            if (drawData.tileCache.TileFrameX % 48 == 0 && drawData.tileCache.TileFrameY % 48 == 0) {
                SpecialTileRenderer.Add(i, j, TileRenderLayer.PostDrawMasterRelics);
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
            tileFrameX += 54;
        }

        public void Render(int i, int j, byte layer) {
            var portalPosition = new Vector2(i * 16f + 24f, j * 16f + 16f) - Main.screenPosition;
            float portalAnimation = Main.GlobalTimeWrappedHourly * 5f;
            float portalSquishMagnitude = 0.1f;
            var portalHoveringPosition = portalPosition + new Vector2(0f, Helper.Wave(Main.GlobalTimeWrappedHourly, -2f, 2f));

            Main.spriteBatch.Draw(AequusTextures.OblivionCraftingStationTile_Portal, portalHoveringPosition, null, Color.White, 0f, AequusTextures.OblivionCraftingStationTile_Portal.Size() /2f, new Vector2(1f + MathF.Sin(portalAnimation) * portalSquishMagnitude, 1f + MathF.Cos(portalAnimation) * portalSquishMagnitude), SpriteEffects.None, 0f);

            var stickTexture = AequusTextures.OblivionCraftingStationTile_Stick;
            var stickFrame = stickTexture.Frame(verticalFrames: 10, frameY: (int)Main.GameUpdateCount / 6 % 10);
            stickFrame.Height -= 2;
            var stickOrigin = stickFrame.Size() / 2f;
            var stickColor = Lighting.GetColor(i + 1, j + 1);
            var stickPositionOffset = new Vector2(17f, 1f);
            Main.spriteBatch.Draw(AequusTextures.OblivionCraftingStationTile_Stick, portalPosition + stickPositionOffset, stickFrame, stickColor, 0f, stickOrigin, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0f);
            Main.spriteBatch.Draw(AequusTextures.OblivionCraftingStationTile_Stick, portalPosition - stickPositionOffset, stickFrame, stickColor, 0f, stickOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}