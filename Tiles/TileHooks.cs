using Aequus.Common.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Drawing;

namespace Aequus.Tiles {
    public class TileHooks
    {
        public interface IGetLightOverride {
            Color GetLightOverride(TileDrawing self, int j, int i, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight);
        }

        public interface IGetTileDrawData {
            void GetTileDrawData(TileDrawing self, int x, int y, Tile tileCache, ushort typeCache, ref short tileFrameX, ref short tileFrameY, ref int tileWidth, ref int tileHeight, ref int tileTop, ref int halfBrickHeight, ref int addFrX, ref int addFrY, ref SpriteEffects tileSpriteEffect, ref Texture2D glowTexture, ref Rectangle glowSourceRect, ref Color glowColor);
        }

        public interface IOnPlaceTile
        {
            bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style);
        }

        /// <summary>
        /// Provides a method to be called when recieving the <see cref="PacketUniqueTileInteraction"/> packet. Allowing for effects to be synced on the network easier.
        /// </summary>
        public interface IUniqueTileInteractions
        {
            void Interact(Player player, int i, int j);
        }
    }
}