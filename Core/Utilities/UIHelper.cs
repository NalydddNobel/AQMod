using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.Core.Utilities;
public static class UIHelper {
    // TODO: Make this actually check the item slot contexts
    public static bool CurrentlyDrawingHotbarSlot => !Main.playerInventory;

    /// <summary>
    /// Draws something with its position set to the center of the inventory slot.
    /// Only use in Pre/PostDraw hooks for items.
    /// </summary>
    public static void InventoryDrawCentered(SpriteBatch spriteBatch, Texture2D texture, Vector2 itemPosition, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects = SpriteEffects.None, Vector2 offset = default) {
        spriteBatch.Draw(texture, itemPosition + offset, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }
}