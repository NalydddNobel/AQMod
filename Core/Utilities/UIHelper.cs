using Aequus.Common.UI;

namespace Aequus.Core.Utilities;

public static class UIHelper {
    public const System.Int32 LeftInventoryPosition = 20;
    // TODO: Make this actually check the item slot contexts
    public static System.Boolean CurrentlyDrawingHotbarSlot => !Main.playerInventory;

    public static void DrawUIPanel(SpriteBatch sb, Texture2D texture, Rectangle rect, Color color = default(Color)) {
        Utils.DrawSplicedPanel(sb, texture, rect.X, rect.Y, rect.Width, rect.Height, 10, 10, 10, 10, color == default ? Color.White : color);
    }

    /// <summary>
    /// Draws something with its position set to the center of the inventory slot.
    /// Only use in Pre/PostDraw hooks for items.
    /// </summary>
    public static void InventoryDrawCentered(SpriteBatch spriteBatch, Texture2D texture, Vector2 itemPosition, Rectangle? frame, Color color, System.Single rotation, Vector2 origin, System.Single scale, SpriteEffects spriteEffects = SpriteEffects.None, Vector2 offset = default) {
        spriteBatch.Draw(texture, itemPosition + offset, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }

    public static void HoverItem(Item item, System.Int32 context = -1) {
        Main.hoverItemName = item.Name;
        Main.HoverItem = item.Clone();
        Main.HoverItem.tooltipContext = context;
    }

    public static System.Int32 BottomLeftInventoryX(System.Boolean ignoreCreative = false) {
        System.Int32 left = LeftInventoryPosition;
        if (!ignoreCreative && Main.LocalPlayer.difficulty == 3 && !Main.CreativeMenu.Blocked) {
            left += 48;
        }
        return UISystem.bottomLeftInventoryOffsetX + left;
    }
}