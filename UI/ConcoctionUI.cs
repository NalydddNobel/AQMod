using AQMod.Items.Potions.Concoctions;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AQMod.UI
{
    public sealed class ConcoctionUI : UIState
    {
        public static InvSlotData potionSlot;
        public static InvSlotData concoctionSlot;
        public static int concocting;
        public static int concoctingMax;
        public static int concoctingTime;
        public static bool concoctTick;
        public static bool slowMode;
        public static bool Active { get; private set; }

        public static Color PotionBGColor => new Color(128, 70, 70, 0) * 0.7f;
        public static Color ChestBGColor => new Color(222, 200, 0, 40) * 0.7f;
        public static Color CococtionItemBGColor => new Color(50, 128, 50, 0) * 0.7f;

        public override void OnInitialize()
        {
            Active = true;
            potionSlot = new InvSlotData(x: 20, y: 270, canSwap: PotionSwap);
            concoctionSlot = new InvSlotData(x: potionSlot.X + (int)(112f * 0.8f), y: potionSlot.Y, canSwap: ConcoctionSwap);
        }

        public override void OnDeactivate()
        {
            Active = false;
            if (potionSlot.Item == null)
            {
                potionSlot.Item = new Item();
            }
            if (!potionSlot.Item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(potionSlot.Item, potionSlot.Item.stack);
                potionSlot.Item.TurnToAir();
            }
            if (concoctionSlot.Item == null)
            {
                concoctionSlot.Item = new Item();
            }
            if (!concoctionSlot.Item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(concoctionSlot.Item, concoctionSlot.Item.stack);
                concoctionSlot.Item.TurnToAir();
            }
        }

        private void ConsumeSlots()
        {
            potionSlot.Item.stack--;
            concoctionSlot.Item.stack--;
            if (potionSlot.Item.stack <= 0)
            {
                concocting = 0;
                potionSlot.Item.TurnToAir();
            }
            if (concoctionSlot.Item.stack <= 0)
            {
                concocting = 0;
                concoctionSlot.Item.TurnToAir();
            }
        }
        private void CheckSlowmode()
        {
            if (!slowMode)
            {
                var inv = Main.LocalPlayer.inventory;
                int amtAir = 0;
                for (int i = 0; i < Main.maxInventory; i++)
                {
                    if (inv[i].IsAir)
                    {
                        amtAir++;
                    }
                }
                if (amtAir <= 1)
                {
                    slowMode = true;
                    concoctingTime = -20;
                }
            }
        }
        private bool SpawnConcoction()
        {
            concoctingTime++;
            Main.PlaySound(SoundID.Item86);

            if (AQUtils.ChestItem(concoctionSlot.Item))
            {
                if (potionSlot.Item.modItem is PotionofContainers)
                {
                    var item = AQItem.GetDefault(ModContent.ItemType<PotionofContainersTag>());
                    var containers = item.modItem as PotionofContainersTag;
                    containers.chest = concoctionSlot.Item.Clone();
                    containers.chest.stack = 1;
                    Main.LocalPlayer.QuickSpawnClonedItem(item);
                    CheckSlowmode();
                    ConsumeSlots();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var item = AQMod.Concoctions.RecipeData[concoctionSlot.Item.type].GetItem(potionSlot.Item, concoctionSlot.Item);
                if (item != null)
                {
                    Main.LocalPlayer.QuickSpawnClonedItem(item);
                    CheckSlowmode();
                    ConsumeSlots();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Active = true;
            if (concocting == 0 && !Main.LocalPlayer.IsTalkingTo<Memorialist>())
            {
                AQMod.Instance.NPCTalkUI.SetState(null);
            }
            else
            {
                if (concocting > 0)
                {
                    concocting--;
                    if (concocting == 0)
                    {
                        concocting = Math.Max(12 - concoctingTime / (slowMode ? 16 : 8) * 2, 4);
                        concoctingMax = concocting;
                        if (!SpawnConcoction())
                        {
                            concoctingTime = 0;
                        }
                    }
                }
                else
                {
                    concoctingTime = 0;
                    concoctingMax = 0;
                    slowMode = false;
                }
                potionSlot.Update();
                concoctionSlot.Update();
            }
        }

        private void DrawCauldron(Vector2 position)
        {
            if (concocting > 0)
            {
                Main.instance.LoadTiles(TileID.CookingPots);
                var texture = Main.tileTexture[TileID.CookingPots];
                var frame = new Rectangle(texture.Width / 2, (int)(Main.GameUpdateCount % 24 / 6) * 36, 16, 16);
                position.X -= 16f * 0.8f;
                position.Y -= 16f * 0.8f;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var realFrame = new Rectangle(frame.X + i * 18, frame.Y + j * 18, frame.Width, frame.Height);
                        Main.spriteBatch.Draw(texture, new Vector2(position.X + i * 16f * 0.8f, position.Y + j * 16f * 0.8f), realFrame, new Color(255, 255, 255, 255), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                    }
                }
            }
            else
            {
                var texture = Main.itemTexture[ItemID.Cauldron];
                Main.spriteBatch.Draw(texture, new Vector2(position.X, position.Y), null, new Color(255, 255, 255, 255), 0f, texture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            var player = Main.LocalPlayer;
            Main.HidePlayerCraftingMenu = true;
            int invBGWidth = (int)(Main.inventoryBackTexture.Width * 0.8f);
            int invBGHeight = (int)(Main.inventoryBackTexture.Height * 0.8f);
            Main.spriteBatch.Draw(Main.inventoryBack5Texture, new Vector2(potionSlot.X, potionSlot.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.inventoryBack5Texture, new Vector2(concoctionSlot.X, concoctionSlot.Y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), 0.8f, SpriteEffects.None, 0f);

            int textX = (potionSlot.X + concoctionSlot.X) / 2 + (int)(Main.inventoryBackTexture.Width * 0.8f) / 2;
            int textY = potionSlot.Y + (int)(Main.inventoryBackTexture.Height * 0.8f) + 8;

            DrawCauldron(new Vector2(textX, potionSlot.Y + Main.inventoryBackTexture.Height * 0.8f / 2));

            if (potionSlot.Item != null && !potionSlot.Item.IsAir)
            {
                float oldScale = Main.inventoryScale;
                Main.inventoryScale = 0.8f;
                InvUI.DrawItem(new Vector2(potionSlot.X, potionSlot.Y), potionSlot.Item);
                Main.inventoryScale = oldScale;
            }
            else
            {
                var bgItem = Main.itemTexture[ItemID.PotionStatue];
                Main.spriteBatch.Draw(bgItem, new Vector2(potionSlot.X + invBGWidth / 2f, potionSlot.Y + invBGHeight / 2f), null, new Color(100, 100, 100, 100), 0f, bgItem.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
            }
            if (concoctionSlot.Item != null && !concoctionSlot.Item.IsAir)
            {
                float oldScale = Main.inventoryScale;
                Main.inventoryScale = 0.8f;
                InvUI.DrawItem(new Vector2(concoctionSlot.X, concoctionSlot.Y), concoctionSlot.Item);
                Main.inventoryScale = oldScale;
            }
            else
            {
                var bgItem = Main.itemTexture[ItemID.StarStatue];
                Main.spriteBatch.Draw(bgItem, new Vector2(concoctionSlot.X + invBGWidth / 2f, concoctionSlot.Y + invBGHeight / 2f), null, new Color(100, 100, 100, 100), 0f, bgItem.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
            }

            bool playedConcoctTick = false;

            string text = concocting > 0 ? AQMod.GetText("Memorialist.StopConcocting") : AQMod.GetText("Memorialist.Concoct");
            var font = Main.fontMouseText;
            var measurement = font.MeasureString(text);

            textY += (int)measurement.Y;

            var textColor = new Color(255, 255, 255, 255);
            var textScale = Vector2.One;

            if (concocting <= 0 && Main.mouseX > potionSlot.X && Main.mouseX < potionSlot.X + Main.inventoryBackTexture.Width * 0.8f && Main.mouseY > potionSlot.Y && Main.mouseY < potionSlot.Y + Main.inventoryBackTexture.Height * 0.8f)
            {
                player.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease && potionSlot.CanSwap(potionSlot.Item, Main.mouseItem))
                {
                    Utils.Swap(ref potionSlot.Item, ref Main.mouseItem);
                    Main.PlaySound(SoundID.Grab);
                }
                if (potionSlot.Item != null && !potionSlot.Item.IsAir)
                {
                    InvUI.HoverItem(potionSlot.Item);
                }
            }
            else if (concocting <= 0 && Main.mouseX > concoctionSlot.X && Main.mouseX < concoctionSlot.X + Main.inventoryBackTexture.Width * 0.8f && Main.mouseY > concoctionSlot.Y && Main.mouseY < concoctionSlot.Y + Main.inventoryBackTexture.Height * 0.8f)
            {
                player.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease && concoctionSlot.CanSwap(concoctionSlot.Item, Main.mouseItem))
                {
                    Utils.Swap(ref concoctionSlot.Item, ref Main.mouseItem);
                    Main.PlaySound(SoundID.Grab);
                }
                if (concoctionSlot.Item != null && !concoctionSlot.Item.IsAir)
                {
                    InvUI.HoverItem(concoctionSlot.Item);
                }
            }
            else
            {
                var hoverBox = Utils.CenteredRectangle(new Vector2(textX, textY), measurement);

                if (hoverBox.Contains(Main.mouseX, Main.mouseY))
                {
                    player.mouseInterface = true;
                    textColor = Color.Yellow;
                    textScale *= 1.1f;
                    if (!concoctTick)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                    }
                    playedConcoctTick = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (!potionSlot.Item.IsAir && !concoctionSlot.Item.IsAir)
                        {
                            if (concocting > 0)
                            {
                                Main.PlaySound(SoundID.Item87);
                                concocting = 0;
                            }
                            else
                            {
                                Main.PlaySound(AQMod.Instance.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/concoction" + Main.rand.Next(2)));
                                if (potionSlot.Item.stack == 1 || concoctionSlot.Item.stack == 1)
                                {
                                    concocting = 1;
                                }
                                else
                                {
                                    concocting = 30;
                                }
                            }
                        }
                    }
                }
            }

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(textX, textY), textColor, 0f, measurement / 2f, textScale);

            concoctTick = playedConcoctTick;
        }

        private static bool PotionSwap(Item slotItem, Item mouseItem)
        {
            if (mouseItem != null && !mouseItem.IsAir &&
                (AQMod.Concoctions.ConcoctiblePotion(mouseItem) || mouseItem.modItem is PotionofContainers))
                return true;
            if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
                return true;
            return false;
        }
        private static bool ConcoctionSwap(Item slotItem, Item mouseItem)
        {
            if (mouseItem != null && !mouseItem.IsAir &&
                (AQMod.Concoctions.RecipeData.ContainsKey(mouseItem.type) || AQUtils.ChestItem(mouseItem)))
                return true;
            if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
                return true;
            return false;
        }
    }
}