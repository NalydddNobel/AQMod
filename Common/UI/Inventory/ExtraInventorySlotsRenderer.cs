using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.UI.Inventory;

public class ExtraInventorySlotsRenderer : UILayer {
    public override string Layer => InterfaceLayers.Inventory_28;

    public Vector2 GetSlotPosition(int index) {
        return new Vector2(524f + index / 5 * 56f * Main.inventoryScale, 42f + index % 5 * 56f * Main.inventoryScale);
    }

    public override bool Draw(SpriteBatch spriteBatch) {
        if (!Main.playerInventory) {
            return true;
        }
        var player = Main.LocalPlayer;
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequusPlayer)) {
            return true;
        }
        Main.inventoryScale = 0.85f;
        var slotTexture = TextureAssets.InventoryBack.Value;
        var slotOrigin = slotTexture.Size() / 2f;
        float slotsDrawn = 0f;
        var slotColor = Main.inventoryBack with { G = (byte)Math.Clamp(Main.inventoryBack.G / 2f, byte.MinValue, byte.MaxValue), } * 1.2f;
        for (; slotsDrawn < InventoryUISystem.ExtraInventorySlotsToRender; slotsDrawn++) {
            int i = (int)slotsDrawn;
            if (!aequusPlayer.extraInventory.IndexInRange(i) || aequusPlayer.extraInventory[i] == null) {
                continue;
            }

            var position = GetSlotPosition(i);

            Main.spriteBatch.Draw(aequusPlayer.extraInventory[i].favorited ? TextureAssets.InventoryBack10.Value : (aequusPlayer.extraInventory[i].newAndShiny ? TextureAssets.InventoryBack15.Value : slotTexture), position, null, slotColor, 0f, slotOrigin, Main.inventoryScale, SpriteEffects.None, 0f);

            position -= slotOrigin * Main.inventoryScale;

            int context = ItemSlot.Context.InventoryItem;
            if (Main.mouseX >= position.X && Main.mouseX <= position.X + slotTexture.Width * Main.inventoryScale && Main.mouseY >= position.Y && Main.mouseY <= position.Y + slotTexture.Height * Main.inventoryScale && !PlayerInput.IgnoreMouseInterface) {
                player.mouseInterface = true;
                ItemSlot.OverrideHover(aequusPlayer.extraInventory, context, i);
                ItemSlot.LeftClick(aequusPlayer.extraInventory, context, i);
                ItemSlot.RightClick(aequusPlayer.extraInventory, context, i);
                if (Main.mouseLeftRelease && Main.mouseLeft) {
                    Recipe.FindRecipes();
                }
                ItemSlot.MouseHover(aequusPlayer.extraInventory, context, i);
            }
            ItemSlotRenderer.DrawFullItem(aequusPlayer.extraInventory[i], context, i, Main.spriteBatch, position, position + slotOrigin * Main.inventoryScale, Main.inventoryScale, 32f, Color.White, Color.White);
        }
        if (InventoryUISystem.ExtraInventorySlotAnimation > 0f) {
            var position = GetSlotPosition((int)slotsDrawn);
            slotsDrawn += InventoryUISystem.ExtraInventorySlotAnimation;
            float rotation = 0f; /* InventoryUISystem.ExtraInventorySlotAnimation * MathHelper.TwoPi */
            Main.spriteBatch.Draw(slotTexture, position, null, slotColor * InventoryUISystem.ExtraInventorySlotAnimation, rotation, slotOrigin, Main.inventoryScale * InventoryUISystem.ExtraInventorySlotAnimation, SpriteEffects.None, 0f);
        }
        if (slotsDrawn > 0f) {
            float opacity = 1f;
            if (aequusPlayer.extraInventorySlots == 0) {
                opacity = Math.Min(InventoryUISystem.CoinsAmmoOffsetX / 50f, 1f);
            }
            else {
                opacity = MathF.Min(slotsDrawn / 2.5f, 1f);
            }
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(496f, 22f), new(0,0,1,1), new Color(Main.inventoryBack.R / 255f * 0.66f, Main.inventoryBack.G / 255f * 0.8f, Main.inventoryBack.B / 255f, Main.inventoryBack.A / 255f) * 0.5f * opacity, 0f, Vector2.Zero, new Vector2(2f, 230f), SpriteEffects.None, 0f);

            var backpackText = "Backpack";
            var textOrigin = ChatManager.GetStringSize(FontAssets.MouseText.Value, backpackText, Vector2.One);
            ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, backpackText, new Vector2(470f + 38f * MathF.Pow(opacity, 2f), textOrigin.Y / 2f), Main.inventoryBack * opacity, 0f, new Vector2(0f, textOrigin.Y / 2f), new Vector2(1f, opacity));
        }
        return true;
    }
}