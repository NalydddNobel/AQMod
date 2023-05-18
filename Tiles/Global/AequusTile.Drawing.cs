using Aequus.Common.DataSets;
using Aequus.Common.Rendering.Tiles;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus {
    public partial class AequusTile {
        private static int TileChange;

        private void Load_Drawing() {
            On_TileDrawing.GetTileDrawTexture_Tile_int_int += On_TileDrawing_GetTileDrawTexture_Tile_int_int;
            On_TileDrawing.DrawMultiTileVinesInWind += On_TileDrawing_DrawMultiTileVinesInWind;
            On_TileDrawing.GetTileDrawData += On_TileDrawing_GetTileDrawData;
            On_TileDrawing.DrawTiles_GetLightOverride += On_TileDrawing_DrawTiles_GetLightOverride;
        }

        private static Color On_TileDrawing_DrawTiles_GetLightOverride(On_TileDrawing.orig_DrawTiles_GetLightOverride orig, TileDrawing self, int j, int i, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight) {
            int type = TileChange > 0 ? TileChange : typeCache;
            if (type >= TileID.Count && TileLoader.GetTile(type) is TileHooks.IGetLightOverride getLightOverride) {
                return getLightOverride.GetLightOverride(self, i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight);
            }
            return orig(self, i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight);
        }

        private static void On_TileDrawing_GetTileDrawData(On_TileDrawing.orig_GetTileDrawData orig, TileDrawing self, int x, int y, Tile tileCache, ushort typeCache, ref short tileFrameX, ref short tileFrameY, out int tileWidth, out int tileHeight, out int tileTop, out int halfBrickHeight, out int addFrX, out int addFrY, out SpriteEffects tileSpriteEffect, out Texture2D glowTexture, out Rectangle glowSourceRect, out Color glowColor) {
            orig(self, x, y, tileCache, typeCache, ref tileFrameX, ref tileFrameY, out tileWidth, out tileHeight, out tileTop, out halfBrickHeight, out addFrX, out addFrY, out tileSpriteEffect, out glowTexture, out glowSourceRect, out glowColor);
            int type = TileChange > 0 ? TileChange : typeCache;
            if (type >= TileID.Count && TileLoader.GetTile(type) is TileHooks.IGetTileDrawData getTileDrawData) {
                getTileDrawData.GetTileDrawData(self, x, y, tileCache, typeCache, ref tileFrameX, ref tileFrameY, ref tileWidth, ref tileHeight, ref tileTop, ref halfBrickHeight, ref addFrX, ref addFrY, ref tileSpriteEffect, ref glowTexture, ref glowSourceRect, ref glowColor);
            }
        }

        private static void On_TileDrawing_DrawMultiTileVinesInWind(On_TileDrawing.orig_DrawMultiTileVinesInWind orig, TileDrawing self, Vector2 screenPosition, Vector2 offSet, int topLeftX, int topLeftY, int sizeX, int sizeY) {
            if (SpecialTileRenderer.ModHangingVines.TryGetValue(Main.tile[topLeftX, topLeftY].TileType, out int value))
                sizeY = value;

            var t = Main.tile[topLeftX, topLeftY];
            var type = t.TileType;
            int renderConversion = TileSets.TileRenderConversion[t.TileType];
            if (renderConversion > 0) {
                TileChange = t.TileType;
                for (int i = topLeftX; i < topLeftX  + sizeX; i++) {
                    for (int j = topLeftY; j < topLeftY + sizeY; j++) {
                        Main.tile[i, j].TileType = (ushort)renderConversion;
                    }
                }
            }

            orig(self, screenPosition, offSet, topLeftX, topLeftY, sizeX, sizeY);

            if (TileChange > 0) {
                for (int i = topLeftX; i < topLeftX + sizeX; i++) {
                    for (int j = topLeftY; j < topLeftY + sizeY; j++) {
                        Main.tile[i, j].TileType = (ushort)TileChange;
                    }
                }
                TileChange = 0;
            }
        }

        private static Texture2D On_TileDrawing_GetTileDrawTexture_Tile_int_int(On_TileDrawing.orig_GetTileDrawTexture_Tile_int_int orig, TileDrawing self, Tile tile, int tileX, int tileY) {
            if (TileChange > 0) {
                tile.TileType = (ushort)TileChange;
            }
            return orig(self, tile, tileX, tileY);
        }
    }
}