using Terraria.DataStructures;

namespace Aequus.Common.Utilities.Extensions;

public static class PlayerDrawSetExtensions {
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, int shader, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth) with { shader = shader });
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, int shader, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth) with { shader = shader });
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float inactiveLayerDepth = 0f) {
        drawInfo.DrawDataCache.Add(new DrawData(texture, position, sourceRect, color, rotation, origin, scale, effect, inactiveLayerDepth));
    }
    public static void Draw(this ref PlayerDrawSet drawInfo, DrawData drawData) {
        drawInfo.DrawDataCache.Add(drawData);
    }
}
