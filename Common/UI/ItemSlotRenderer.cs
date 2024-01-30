using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.UI;

public class ItemSlotRenderer {
    public const Int32 InventoryBackFramesX = 1;
    public static Int32 InventoryBackFramesY = 3;

    public static Rectangle InventoryBackFrame(Texture2D back, Int32 frameX = 0, Int32 frameY = 0) {
        return back.Frame(horizontalFrames: InventoryBackFramesX, verticalFrames: InventoryBackFramesY, frameX: frameX, frameY: frameY);
    }

    public struct ItemDrawData {
        public Item item;
        public Texture2D texture;
        public Vector2 drawPos;
        public Vector2 origin;
        public Rectangle frame;
        public Color drawColor;
        public Color color;
        public Single scale;
        public Single scale2;
    }

    [Obsolete("Vanilla now draws items differently. This method is now out of date.")]
    public static ItemDrawData GetDrawData(Item item, Vector2 position, Color? color = null, Int32 maxSize = 32) {
        Main.instance.LoadItem(item.type);

        var data = new ItemDrawData {
            item = item,
            texture = TextureAssets.Item[item.type].Value
        };

        data.frame = Main.itemAnimations[item.type] == null ? data.texture.Frame() : Main.itemAnimations[item.type].GetFrame(data.texture);
        data.scale2 = 1f;
        data.color = Color.White;

        ItemSlot.GetItemLight(ref data.color, ref data.scale2, item.type);

        data.drawColor = item.GetAlpha(data.color);
        if (color != null) {
            Single r = data.drawColor.R / 255f;
            Single g = data.drawColor.G / 255f;
            Single b = data.drawColor.B / 255f;
            Single a = data.drawColor.A / 255f;

            var color2 = color.Value;
            Single r2 = color2.R / 255f;
            Single g2 = color2.G / 255f;
            Single b2 = color2.B / 255f;
            Single a2 = color2.A / 255f;

            data.drawColor = new Color(r * r2, g * g2, b * b2, a * a2);
        }

        Single scale = 1f;
        if (data.frame.Width > maxSize || data.frame.Height > maxSize) {
            scale = data.frame.Width <= data.frame.Height ? (Single)maxSize / data.frame.Height : (Single)maxSize / data.frame.Width;
        }

        data.scale = scale * Main.inventoryScale;

        Vector2 backSize = TextureAssets.InventoryBack.Value.Size() * Main.inventoryScale;
        data.origin = data.frame.Size() / 2f;

        data.drawPos = position + backSize / 2f;
        //DrawItemInv(data.drawPos, item, data.frame, data.drawColor, data.color, data.origin, data.scale, data.scale2);
        return data;
    }

    public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Item item, Rectangle frame, Color drawColor, Color lightColor, Vector2 origin, Single scale, Single scale2) {
        Texture2D itemTexture = texture;
        if (ItemLoader.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, item.GetColor(Main.inventoryBack), origin, scale * scale2)) {
            spriteBatch.Draw(itemTexture, position, frame, drawColor, 0f, origin, scale * scale2, SpriteEffects.None, 0f);
            if (item.color != Color.Transparent)
                spriteBatch.Draw(itemTexture, position, frame, item.GetColor(Main.inventoryBack), 0f, origin, scale * scale2, SpriteEffects.None, 0f);
        }
        ItemLoader.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, item.GetColor(lightColor), origin, scale * scale2);
        if (ItemID.Sets.TrapSigned[item.type])
            spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * Main.inventoryScale, new Rectangle(4, 58, 8, 8), Main.inventoryBack, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
    }
    public static void Draw(SpriteBatch spriteBatch, Item item, Vector2 position, Color? color = null, Int32 maxSize = 32) {
        Draw(spriteBatch, GetDrawData(item, position, color, maxSize));
    }
    public static void Draw(SpriteBatch spriteBatch, ItemDrawData data) {
        Draw(spriteBatch, data.texture, data.drawPos, data.item, data.frame, data.drawColor, data.color, data.origin, data.scale, data.scale2);
    }

    public static void DrawFullItem(Item item, Int32 context, Int32 slot, SpriteBatch spriteBatch, Vector2 position, Vector2 itemCenter, Single scale, Single sizeLimit, Color itemDrawColor, Color color) {
        ItemSlot.DrawItemIcon(item, context, spriteBatch, itemCenter, scale, sizeLimit, itemDrawColor);

        if (ItemID.Sets.TrapSigned[item.type]) {
            spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * Main.inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
        }

        if (ItemID.Sets.DrawUnsafeIndicator[item.type]) {
            Vector2 vector2 = new Vector2(-4f, -4f) * scale;
            Texture2D value6 = TextureAssets.Extra[258].Value;
            Rectangle rectangle2 = value6.Frame();
            spriteBatch.Draw(value6, position + vector2 + new Vector2(40f, 40f) * scale, rectangle2, color, 0f, rectangle2.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        // Rubblemaker Icons
        if (item.type == ItemID.RubblemakerSmall || item.type == ItemID.RubblemakerMedium || item.type == ItemID.RubblemakerLarge) {
            Vector2 vector3 = new Vector2(2f, -6f) * scale;
            switch (item.type) {
                case ItemID.RubblemakerSmall: {
                        Texture2D value9 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle5 = value9.Frame(3, 1, 2);
                        spriteBatch.Draw(value9, position + vector3 + new Vector2(40f, 40f) * scale, rectangle5, color, 0f, rectangle5.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case ItemID.RubblemakerMedium: {
                        Texture2D value8 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle4 = value8.Frame(3, 1, 1);
                        spriteBatch.Draw(value8, position + vector3 + new Vector2(40f, 40f) * scale, rectangle4, color, 0f, rectangle4.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
                case ItemID.RubblemakerLarge: {
                        Texture2D value7 = TextureAssets.Extra[257].Value;
                        Rectangle rectangle3 = value7.Frame(3);
                        spriteBatch.Draw(value7, position + vector3 + new Vector2(40f, 40f) * scale, rectangle3, color, 0f, rectangle3.Size() / 2f, 1f, SpriteEffects.None, 0f);
                        break;
                    }
            }
        }

        if (item.stack > 1) {
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(), position + new Vector2(10f, 26f) * scale, color, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
        }

        Int32 ammoCount = -1;
        if (context == 13) {
            if (item.DD2Summon) {
                for (Int32 i = 0; i < Main.InventorySlotsTotal; i++) {
                    if (Main.LocalPlayer.inventory[i].type == ItemID.DD2EnergyCrystal) {
                        ammoCount += Main.LocalPlayer.inventory[i].stack;
                    }
                }

                if (ammoCount >= 0) {
                    ammoCount++;
                }
            }

            if (item.useAmmo > 0) {
                ammoCount = 0;
                for (Int32 j = 0; j < Main.InventorySlotsTotal; j++) {
                    if (Main.LocalPlayer.inventory[j].stack > 0 && ItemLoader.CanChooseAmmo(item, Main.LocalPlayer.inventory[j], Main.LocalPlayer)) {
                        ammoCount += Main.LocalPlayer.inventory[j].stack;
                    }
                }
            }

            if (item.fishingPole > 0) {
                ammoCount = 0;
                for (Int32 k = 0; k < Main.InventorySlotsTotal; k++) {
                    if (Main.LocalPlayer.inventory[k].bait > 0) {
                        ammoCount += Main.LocalPlayer.inventory[k].stack;
                    }
                }
            }

            if (item.tileWand > 0) {
                Int32 tileWand = item.tileWand;
                ammoCount = 0;
                for (Int32 l = 0; l < Main.InventorySlotsTotal; l++) {
                    if (Main.LocalPlayer.inventory[l].type == tileWand) {
                        ammoCount += Main.LocalPlayer.inventory[l].stack;
                    }
                }
            }

            if (item.type == ItemID.Wrench || item.type == ItemID.GreenWrench || item.type == ItemID.BlueWrench || item.type == ItemID.YellowWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.WireKite) {
                ammoCount = 0;
                for (Int32 m = 0; m < Main.InventorySlotsTotal; m++) {
                    if (Main.LocalPlayer.inventory[m].type == ItemID.Wire) {
                        ammoCount += Main.LocalPlayer.inventory[m].stack;
                    }
                }
            }
        }

        if (ammoCount != -1) {
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, ammoCount.ToString(), position + new Vector2(8f, 30f) * scale, color, 0f, Vector2.Zero, new Vector2(scale * 0.8f), -1f, scale);
        }

        if (context == 13) {
            String text = String.Concat(slot + 1);
            if (text == "10") {
                text = "0";
            }

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, position + new Vector2(8f, 4f) * scale, color, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
        }

        if (context == 13 && item.potion) {
            Color color3 = item.GetAlpha(color) * (Main.LocalPlayer.potionDelay / (Single)Main.LocalPlayer.potionDelayTime);
            spriteBatch.Draw(TextureAssets.Cd.Value, itemCenter, null, color3, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
        }

        if ((Math.Abs(context) == 10 || context == 18) && ((item.expertOnly && !Main.expertMode) || (item.masterOnly && !Main.masterMode))) {
            Vector2 position3 = itemCenter;
            Color white = Color.White;
            spriteBatch.Draw(TextureAssets.Cd.Value, position3, null, white, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
        }
    }

    public static void DrawUIBack(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle itemFrame, Single itemScale, Color color, Single progress = 1f) {
        var backFrame = InventoryBackFrame(texture, frameY: 0);
        Int32 frameY = (Int32)(backFrame.Height * progress);
        var uiFrame = new Rectangle(0, backFrame.Height - frameY, backFrame.Width, frameY);
        position.Y += (backFrame.Height - frameY) * Main.inventoryScale;
        spriteBatch.Draw(texture, position, uiFrame, color, 0f, backFrame.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
    }
}