using AQMod.Items;
using AQMod.NPCs.Friendly;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AQMod.UI
{
    public sealed class RenameItemUI : UIState
    {
        private static bool initItemSlot;
        private static Textbox textBox;
        private static InvSlotData slot;

        public override void OnInitialize()
        {
            initItemSlot = false;
            slot = new InvSlotData(x: 20, y: 270, canSwap: CanSwapItem);
            textBox = new Textbox(x: slot.X, y: slot.Y + (int)(Main.inventoryBack3Texture.Height * 0.8f) + 4, 440, 32, color: new Color(50, 106, 46, 255), maxText: 64, textOffsetY: 5);
            Append(textBox);
        }

        public override void OnDeactivate()
        {
            textBox.text = "";
            if (slot.Item == null)
            {
                slot.Item = new Item();
            }
            if (!slot.Item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(slot.Item, slot.Item.stack);
                slot.Item.TurnToAir();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<BalloonMerchant>())
            {
                AQMod.GetInstance().NPCTalkState.SetState(null);
            }
            else
            {
                slot.Update();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var player = Main.LocalPlayer;
            Main.HidePlayerCraftingMenu = true;
            int slotX = slot.X + (int)(56f * 0.8f);
            Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/UI/RenameItem"), new Vector2(slot.X, slot.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.inventoryBack3Texture, new Vector2(slotX, slot.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            bool hover;
            bool hover2 = Main.mouseX > slot.X && Main.mouseX < slot.X + Main.inventoryBackTexture.Width * 0.8f && Main.mouseY > slot.Y && Main.mouseY < slot.Y + Main.inventoryBackTexture.Height * 0.8f;
            if (hover2)
            {
                Main.HoverItem = new Item();
                Main.hoverItemName = Language.GetTextValue("Mods.AQMod.BalloonMerchant.RenameItem.PressHere");
                player.mouseInterface = true;
                hover = false;
            }
            else
            {
                hover = Main.mouseX > slotX && Main.mouseX < slotX + Main.inventoryBackTexture.Width * 0.8f && Main.mouseY > slot.Y && Main.mouseY < slot.Y + Main.inventoryBackTexture.Height * 0.8f;
                if (hover)
                {
                    player.mouseInterface = true;
                }
            }
            if (hover && Main.mouseLeft && Main.mouseLeftRelease && slot.CanSwap(slot.Item, Main.mouseItem))
            {
                Utils.Swap(ref slot.Item, ref Main.mouseItem);
                Main.PlaySound(SoundID.Grab);
                initItemSlot = false;
            }
            if (slot.Item != null && !slot.Item.IsAir)
            {
                slot.Update();
                if (!initItemSlot)
                {
                    textBox.text = slot.Item.Name;
                    initItemSlot = true;
                }
                int price = NameTagItem.RenamePrice(slot.Item);
                if (hover2)
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease && price != -1)
                    {
                        if (player.CanBuyItem(price, customCurrency: -1))
                        {
                            if (textBox.text != "" && textBox.text != slot.Item.Name)
                            {
                                player.BuyItem(price, -1);
                                AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/Select", 0.5f);
                                var nameTagItem = slot.Item.GetGlobalItem<NameTagItem>();
                                string itemName = textBox.text;
                                if (string.IsNullOrWhiteSpace(itemName))
                                {
                                    itemName = "";
                                }
                                nameTagItem.nameTag = itemName;
                                nameTagItem.timesRenamed++;
                                nameTagItem.UpdateName(slot.Item);
                                textBox.text = "";
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
                    InvUI.HoverItem(slot.Item);
                }
                //textBox.Draw(Main.spriteBatch);
                float oldScale = Main.inventoryScale;
                Main.inventoryScale = 0.8f;
                InvUI.DrawItem(new Vector2(slotX, slot.Y), slot.Item);
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
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, costText, new Vector2(slotX + 50, slot.Y), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, coinsText, new Vector2(slotX + 50 + Main.fontMouseText.MeasureString(costText).X, slot.Y), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
            else
            {
                initItemSlot = false;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, Language.GetTextValue("Mods.AQMod.BalloonMerchant.RenameItem.PlaceHere"), new Vector2(slotX + 50, slot.Y), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
        }

        private static bool CanSwapItem(Item slotItem, Item mouseItem)
        {
            if (mouseItem != null && !mouseItem.IsAir && mouseItem.maxStack == 1 && !AQItem.Sets.NoRename.Contains(mouseItem.type))
                return true;
            if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
                return true;
            return false;
        }
    }
}