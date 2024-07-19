using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.GUI;

public class ItemSlotDrawHelper {
    public static void DrawSimple(SpriteBatch spriteBatch, Item item, Vector2 position, int context, float scale = 1f, Color? color = null, float maxSize = 32f) {
        ItemSlot.DrawItemIcon(item, context, spriteBatch, position, scale, maxSize, color ?? Color.White);
    }

    public static void DrawFullItem(Item item, int context, int slot, SpriteBatch spriteBatch, Vector2 position, Vector2 itemCenter, float scale, float sizeLimit, Color itemDrawColor, Color color) {
        ItemSlot.DrawItemIcon(item, context, spriteBatch, itemCenter, scale, sizeLimit, itemDrawColor);

        if (ItemID.Sets.TrapSigned[item.type]) {
            spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * Main.inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
        }

        if (ItemID.Sets.DrawUnsafeIndicator[item.type]) {
            Texture2D unsafeIndicatorTexture = TextureAssets.Extra[ExtrasID.UnsafeIndicator].Value;
            Rectangle frame = unsafeIndicatorTexture.Frame();
            spriteBatch.Draw(unsafeIndicatorTexture, position + new Vector2(36f, 36f) * scale, frame, color, 0f, frame.Size() / 2f, 1f, SpriteEffects.None, 0f);
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

        int ammoCount = -1;
        if (context == 13) {
            if (item.DD2Summon) {
                for (int i = 0; i < Main.InventorySlotsTotal; i++) {
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
                for (int j = 0; j < Main.InventorySlotsTotal; j++) {
                    if (Main.LocalPlayer.inventory[j].stack > 0 && ItemLoader.CanChooseAmmo(item, Main.LocalPlayer.inventory[j], Main.LocalPlayer)) {
                        ammoCount += Main.LocalPlayer.inventory[j].stack;
                    }
                }
            }

            if (item.fishingPole > 0) {
                ammoCount = 0;
                for (int k = 0; k < Main.InventorySlotsTotal; k++) {
                    if (Main.LocalPlayer.inventory[k].bait > 0) {
                        ammoCount += Main.LocalPlayer.inventory[k].stack;
                    }
                }
            }

            if (item.tileWand > 0) {
                int tileWand = item.tileWand;
                ammoCount = 0;
                for (int l = 0; l < Main.InventorySlotsTotal; l++) {
                    if (Main.LocalPlayer.inventory[l].type == tileWand) {
                        ammoCount += Main.LocalPlayer.inventory[l].stack;
                    }
                }
            }

            if (item.type == ItemID.Wrench || item.type == ItemID.GreenWrench || item.type == ItemID.BlueWrench || item.type == ItemID.YellowWrench || item.type == ItemID.MulticolorWrench || item.type == ItemID.WireKite) {
                ammoCount = 0;
                for (int m = 0; m < Main.InventorySlotsTotal; m++) {
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
            string text = string.Concat(slot + 1);
            if (text == "10") {
                text = "0";
            }

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, position + new Vector2(8f, 4f) * scale, color, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
        }

        if (context == 13 && item.potion) {
            Color color3 = item.GetAlpha(color) * (Main.LocalPlayer.potionDelay / (float)Main.LocalPlayer.potionDelayTime);
            spriteBatch.Draw(TextureAssets.Cd.Value, itemCenter, null, color3, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
        }

        if ((Math.Abs(context) == 10 || context == 18) && (item.expertOnly && !Main.expertMode || item.masterOnly && !Main.masterMode)) {
            Vector2 position3 = itemCenter;
            Color white = Color.White;
            spriteBatch.Draw(TextureAssets.Cd.Value, position3, null, white, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
        }
    }
}