﻿using Aequus.Common.UI;
using Aequus.Common.UI.Elements;
using Aequus.Items;
using Aequus.NPCs.Town.SkyMerchantNPC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.UI.Renaming {
    public class RenameItemUIState : UIState {
        public bool initItemSlot;
        public AequusTextboxElement textBox;
        public AequusItemSlotElement slot;

        public static Color CorrectKeyColor = new Color(255, 255, 80, 255);
        public static Color IncorrectKeyColor = new Color(255, 120, 120, 255);

        public override void OnInitialize() {
            initItemSlot = false;
            var back = TextureAssets.InventoryBack3.Value;

            OverrideSamplerState = SamplerState.LinearClamp;

            Width.Set(750, 0f);
            Height.Set(100, 0f);

            slot = new AequusItemSlotElement(back);
            slot.Width.Set(52f, 0f);
            slot.Height.Set(52f, 0f);
            slot.Left.Set(AequusUI.LeftInv, 0f);
            slot.Top.Set(270, 0f);
            slot.Recalculate();

            textBox = new AequusTextboxElement(color: new Color(50, 106, 46, 255), maxText: 64, textOffsetY: 5);
            textBox.Left.Set(slot.Left.Pixels, slot.Left.Percent);
            textBox.Top.Set(slot.Top.Pixels + (int)(back.Height * 0.8f) + 4, slot.Top.Percent);
            textBox.Width.Set(440, 0f);
            textBox.Height.Set(32, 0f);

            Append(textBox);
        }

        public override void OnDeactivate() {
            textBox.text = "";
            slot.item ??= new Item();
            if (!slot.item.IsAir) {
                Main.LocalPlayer.QuickSpawnItem(new EntitySource_WorldEvent(), slot.item, slot.item.stack);
                slot.item.TurnToAir();
            }
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<SkyMerchant>()) {
                Aequus.UserInterface.SetState(null);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);
            var player = Main.LocalPlayer;
            Main.hidePlayerCraftingMenu = true;

            var dimensions = GetDimensions();
            var slotDimensions = slot.GetDimensions();
            int slotX = (int)(slot.Left.GetValue(slotDimensions.Width) + (int)(56f * 0.8f));
            var back = TextureAssets.InventoryBack3.Value;
            Main.spriteBatch.Draw(back, new Vector2(slotDimensions.X, slotDimensions.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(AequusTextures.RenameBackIcon, new Vector2(slotDimensions.X, slotDimensions.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(back, new Vector2(slotX, slotDimensions.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            bool hover;
            bool hover2 = Main.mouseX > slotDimensions.X && Main.mouseX < slotDimensions.X + back.Width * 0.8f && Main.mouseY > slotDimensions.Y && Main.mouseY < slotDimensions.Y + back.Height * 0.8f;
            if (hover2) {
                Main.HoverItem = new Item();
                Main.hoverItemName = TextHelper.GetTextValue("Chat.SkyMerchant.PressHere");
                player.mouseInterface = true;
                hover = false;
            }
            else {
                hover = Main.mouseX > slotX && Main.mouseX < slotX + back.Width * 0.8f && Main.mouseY > slotDimensions.Y && Main.mouseY < slotDimensions.Y + back.Height * 0.8f;
                if (hover) {
                    player.mouseInterface = true;
                }
            }

            if (hover && Main.mouseLeft && Main.mouseLeftRelease && CanSwapItem(slot.item, Main.mouseItem)) {
                Utils.Swap(ref slot.item, ref Main.mouseItem);
                SoundEngine.PlaySound(SoundID.Grab);
                initItemSlot = false;
            }

            var font = FontAssets.MouseText.Value;

            if (slot.HasItem) {
                if (!initItemSlot) {
                    var nameTag = slot.item.GetGlobalItem<AequusItem>();
                    textBox.text = nameTag.HasNameTag ? nameTag.NameTag : slot.item.Name;
                    initItemSlot = true;
                }

                int price = AequusItem.GetRenamePrice(slot.item);
                if (hover2) {
                    if (Main.mouseLeft && Main.mouseLeftRelease && price != -1) {
                        if (player.CanAfford(price, customCurrency: -1)) {
                            if (textBox.text != "" && textBox.text != slot.item.Name) {
                                player.BuyItem(price, -1);
                                SoundEngine.PlaySound(AequusSounds.select with { Volume = 0.5f });

                                var nameTag = slot.item.GetGlobalItem<AequusItem>();
                                string itemName = textBox.text;
                                if (string.IsNullOrWhiteSpace(itemName)) {
                                    itemName = "";
                                }
                                nameTag.NameTag = itemName;
                                nameTag.RenameCount++;
                                nameTag.CheckNameTag(slot.item);
                                textBox.text = "";
                            }
                        }
                        else {
                            SoundEngine.PlaySound(AequusSounds.squeak with { Volume = 0.5f });
                        }
                    }
                }
                else if (hover) {
                    AequusUI.HoverItem(slot.item, ItemSlot.Context.ShopItem);
                }
                //textBox.Draw(Main.spriteBatch);
                float oldScale = Main.inventoryScale;
                Main.inventoryScale = 0.8f;
                ItemSlotRenderer.Draw(Main.spriteBatch, slot.item, new Vector2(slotX, slotDimensions.Y));
                Main.inventoryScale = oldScale;

                string costText = Language.GetTextValue("LegacyInterface.46") + ": ";
                string coinsText = "";
                if (price > 0) {
                    int[] coins = Utils.CoinsSplit(price);
                    if (coins[3] > 0) {
                        coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + coins[3] + " " + Language.GetTextValue("LegacyInterface.15") + "] ";
                    }
                    if (coins[2] > 0) {
                        coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + coins[2] + " " + Language.GetTextValue("LegacyInterface.16") + "] ";
                    }
                    if (coins[1] > 0) {
                        coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + coins[1] + " " + Language.GetTextValue("LegacyInterface.17") + "] ";
                    }
                    if (coins[0] > 0) {
                        coinsText = coinsText + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + coins[0] + " " + Language.GetTextValue("LegacyInterface.18") + "] ";
                    }
                }
                else {
                    coinsText = TextHelper.GetTextValue("Misc.PriceFree");
                }
                ItemSlot.DrawSavings(Main.spriteBatch, slotX + 130, Main.instance.invBottom, true);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, costText, new Vector2(slotX + 50, slotDimensions.Y), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, coinsText, new Vector2(slotX + 50 + font.MeasureString(costText).X, slotDimensions.Y), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }
            else {
                initItemSlot = false;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, TextHelper.GetTextValue("Chat.SkyMerchant.PlaceHere"), new Vector2(slotX + 50, slotDimensions.Y), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
            }

            if (textBox.text != null) {
                textBox.ShowText = ChatBoxDecodeText(textBox.text, Main.keyState.IsKeyDown(Keys.LeftControl));
            }
        }

        public static string ChatBoxDecodeText(string text, bool showKeys = false) {
            string newName = "";
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == AequusItem.LanguageKeyChar) {
                    string keyText = "";
                    int j = i + 1;
                    for (; j < text.Length; j++) {
                        if (text[j] == '|') {
                            j++;
                            break;
                        }
                        if (text[j] == ' ') {
                            break;
                        }
                        keyText += text[j];
                    }
                    i = j - 1;
                    var langOrFormatedText = Language.GetText(keyText).FormatWith(Lang.CreateDialogSubstitutionObject());
                    var color = CorrectKeyColor;
                    if (langOrFormatedText == keyText || langOrFormatedText == "") {
                        langOrFormatedText = keyText;
                        color = IncorrectKeyColor;
                    }
                    if (langOrFormatedText != "") {
                        langOrFormatedText = TextHelper.ColorCommand(showKeys ? keyText : langOrFormatedText, color, alphaPulse: true);
                    }
                    if (text[i] == '|') {
                        langOrFormatedText += TextHelper.ColorCommand("|", Color.Gray, alphaPulse: true);
                    }
                    newName += TextHelper.ColorCommand("$", (color * 0.5f).UseA(color.A)) + langOrFormatedText;
                }
                else {
                    newName += text[i];
                }
            }
            return newName;
        }

        private static bool CanSwapItem(Item slotItem, Item mouseItem) {
            if (mouseItem != null && !mouseItem.IsAir && AequusItem.CanRename(mouseItem))
                return true;
            if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
                return true;
            return false;
        }
    }
}