using AQMod.Common;
using AQMod.Content.UserInterface.ItemSlots;
using AQMod.Items;
using AQMod.Sounds;
using AQMod.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AQMod.Content.NameTags
{
    public sealed class UserInterfaceRenameItem : IAutoloadType
    {
        public static bool IsActive { get; internal set; }
        private static bool initItemSlot;
        private const int SlotX = 20;
        private const int SlotY = 270;
        private static Item item;
        private static UserInterfaceTextboxInstance textUI;

        public static void Draw()
        {
            var player = Main.player[Main.myPlayer];
            if (!IsActive)
            {
                textUI.text = "";
                if (item != null)
                {
                    player.QuickSpawnClonedItem(item);
                    item = null;
                }
                return;
            }
            if (player.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<NPCs.Friendly.BalloonMerchant>())
            {
                IsActive = false;
                return;
            }
            Main.HidePlayerCraftingMenu = true;
            int slotX = SlotX + (int)(56f * 0.8f);
            Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/UI/RenameItem"), new Vector2(SlotX, SlotY), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.inventoryBack3Texture, new Vector2(slotX, SlotY), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            bool hover;
            bool hover2 = Main.mouseX > SlotX && Main.mouseX < SlotX + Main.inventoryBackTexture.Width * 0.8f && Main.mouseY > SlotY && Main.mouseY < SlotY + Main.inventoryBackTexture.Height * 0.8f;
            if (hover2)
            {
                Main.HoverItem = new Item();
                Main.hoverItemName = Language.GetTextValue("Mods.AQMod.BalloonMerchant.RenameItem.PressHere");
                player.mouseInterface = true;
                hover = false;
            }
            else
            {
                hover = Main.mouseX > slotX && Main.mouseX < slotX + Main.inventoryBackTexture.Width * 0.8f && Main.mouseY > SlotY && Main.mouseY < SlotY + Main.inventoryBackTexture.Height * 0.8f;
                if (hover)
                {
                    player.mouseInterface = true;
                }
            }
            if (hover && Main.mouseLeft && Main.mouseLeftRelease && ItemSlotInteractionHelper.CanSwapItem_NameTag(item, Main.mouseItem))
            {
                if (item == null)
                {
                    item = new Item();
                }
                ItemSlotInteractionHelper.SwapItem_NameTag(ref item, ref Main.mouseItem);
                initItemSlot = false;
            }
            if (item != null && !item.IsAir)
            {
                if (!initItemSlot)
                {
                    textUI.text = item.Name;
                    initItemSlot = true;
                }
                int price = NameTagItem.RenamePrice(item);
                if (hover2)
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease && price != -1)
                    {
                        if (player.CanBuyItem(price, customCurrency: -1))
                        {
                            if (textUI.text != "")
                            {
                                player.BuyItem(price, -1);
                                AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/Select", 0.5f);
                                var nameTagItem = item.GetGlobalItem<NameTagItem>();
                                string itemName = textUI.text;
                                if (string.IsNullOrWhiteSpace(itemName))
                                {
                                    itemName = "";
                                }
                                nameTagItem.nameTag = itemName;
                                nameTagItem.timesRenamed++;
                                nameTagItem.UpdateName(item);
                                textUI.text = "";
                            }
                        }
                        else
                        {
                            AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/Mouse", 0.5f);
                        }

                    }
                }
                else if (hover)
                {
                    UserInterfaceUtilities.HoverItem(item);
                }
                textUI.Draw(Main.spriteBatch);
                float oldScale = Main.inventoryScale;
                Main.inventoryScale = 0.8f;
                UserInterfaceUtilities.DrawItemInv(new Vector2(slotX, SlotY), item);
                Main.inventoryScale = oldScale;

                string costText = Language.GetTextValue("LegacyInterface.46") + ": ";
                string coinsText = "";
                int[] coins = Utils.CoinsSplit(price);
                if (coins[3] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + coins[3] + " " + Language.GetTextValue("LegacyInterface.15") + "] ";
                }
                if (coins[2] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + coins[2] + " " + Language.GetTextValue("LegacyInterface.16") + "] ";
                }
                if (coins[1] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + coins[1] + " " + Language.GetTextValue("LegacyInterface.17") + "] ";
                }
                if (coins[0] > 0)
                {
                    coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + coins[0] + " " + Language.GetTextValue("LegacyInterface.18") + "] ";
                }
                ItemSlot.DrawSavings(Main.spriteBatch, slotX + 130, Main.instance.invBottom, true);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, costText, new Vector2(slotX + 50, SlotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, coinsText, new Vector2(slotX + 50 + Main.fontMouseText.MeasureString(costText).X, (float)SlotY), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
            else
            {
                initItemSlot = false;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, Language.GetTextValue("Mods.AQMod.BalloonMerchant.RenameItem.PlaceHere"), new Vector2(slotX + 50, SlotY), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
        }

        void IAutoloadType.OnLoad()
        {
            textUI = new UserInterfaceTextboxInstance(SlotX, SlotY + (int)(Main.inventoryBack3Texture.Height * 0.8f) + 4, 440, 32, color: new Color(50, 106, 46, 255), maxText: 64, textOffsetY: 5);
        }

        void IAutoloadType.Unload()
        {
            textUI = null;
        }
    }
}