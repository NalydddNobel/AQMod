using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;

namespace Aequus.Core.Utilities;
public static class DrawHelper {
    public delegate void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

    public static void DrawLine(Draw draw, Vector2 start, float rotation, float length, float width, Color color) {
        draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, rotation, new Vector2(1f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0f);
    }
    public static void DrawLine(Vector2 start, float rotation, float length, float width, Color color) {
        DrawLine(Main.spriteBatch.Draw, start, rotation, length, width, color);
    }
    public static void DrawLine(Draw draw, Vector2 start, Vector2 end, float width, Color color) {
        DrawLine(draw, start, (start - end).ToRotation(), (end - start).Length(), width, color);
    }
    public static void DrawLine(Vector2 start, Vector2 end, float width, Color color) {
        DrawLine(Main.spriteBatch.Draw, start, end, width, color);
    }

    #region Colors
    public static Color GetStringColor(int stringColorID) {
        if (stringColorID == 27) {
            return Main.DiscoColor;
        }
        return WorldGen.paintColor(stringColorID);
    }
    #endregion

    #region Lighting
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetLightColor(Vector2 worldCoordinates) {
        return Lighting.GetColor(worldCoordinates.ToTileCoordinates());
    }
    #endregion

    #region Begin
    #endregion
}