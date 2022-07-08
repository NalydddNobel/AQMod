using Aequus.Content.PotionConcoctions;
using Aequus.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.UI.States
{
    //public class ConcoctionUI : UIState
    //{
    //    public static ItemSlotElement potionSlot;
    //    public static ItemSlotElement concoctionSlot;
    //    public static int concocting;
    //    public static int concoctingMax;
    //    public static int concoctingTime;
    //    public static bool concoctTick;
    //    public static bool slowMode;
    //    public static bool Active { get; private set; }

    //    public static Color PotionBGColor => new Color(128, 70, 70, 0) * 0.7f;
    //    public static Color ChestBGColor => new Color(222, 200, 0, 40) * 0.7f;
    //    public static Color CococtionItemBGColor => new Color(50, 128, 50, 0) * 0.7f;

    //    public override void OnInitialize()
    //    {
    //        Active = true;
    //        potionSlot = new ItemSlotElement(TextureAssets.InventoryBack.Value);
    //        potionSlot.Left.Set(20, 0f);
    //        potionSlot.Top.Set(270, 0f);
    //        concoctionSlot = new ItemSlotElement(TextureAssets.InventoryBack.Value);
    //        concoctionSlot.Left.Set(potionSlot.Left.Pixels + (int)(112f * 0.8f), 0f);
    //        concoctionSlot.Top.Set(potionSlot.Top.Pixels, 0f);
    //    }

    //    public override void OnDeactivate()
    //    {
    //        Active = false;
    //        if (potionSlot.item == null)
    //        {
    //            potionSlot.item = new Item();
    //        }
    //        if (!potionSlot.item.IsAir)
    //        {
    //            Main.LocalPlayer.QuickSpawnClonedItem(null, potionSlot.item, potionSlot.item.stack);
    //            potionSlot.item.TurnToAir();
    //        }
    //        if (concoctionSlot.item == null)
    //        {
    //            concoctionSlot.item = new Item();
    //        }
    //        if (!concoctionSlot.item.IsAir)
    //        {
    //            Main.LocalPlayer.QuickSpawnClonedItem(null, concoctionSlot.item, concoctionSlot.item.stack);
    //            concoctionSlot.item.TurnToAir();
    //        }
    //    }

    //    private void ConsumeSlots()
    //    {
    //        potionSlot.item.stack--;
    //        concoctionSlot.item.stack--;
    //        if (potionSlot.item.stack <= 0)
    //        {
    //            concocting = 0;
    //            potionSlot.item.TurnToAir();
    //        }
    //        if (concoctionSlot.item.stack <= 0)
    //        {
    //            concocting = 0;
    //            concoctionSlot.item.TurnToAir();
    //        }
    //    }
    //    private void CheckSlowmode()
    //    {
    //        if (!slowMode)
    //        {
    //            var inv = Main.LocalPlayer.inventory;
    //            int amtAir = 0;
    //            for (int i = 0; i < Main.InventorySlotsTotal; i++)
    //            {
    //                if (inv[i].IsAir)
    //                {
    //                    amtAir++;
    //                }
    //            }
    //            if (amtAir <= 1)
    //            {
    //                slowMode = true;
    //                concoctingTime = -20;
    //            }
    //        }
    //    }
    //    private bool SpawnConcoction()
    //    {
    //        concoctingTime++;
    //        SoundEngine.PlaySound(SoundID.Item86);

    //        //if (AQUtils.ChestItem(concoctionSlot.item))
    //        //{
    //        //    if (potionSlot.item.ModItem is PotionofContainers)
    //        //    {
    //        //        var item = AQItem.GetDefault(ModContent.itemType<PotionofContainersTag>());
    //        //        var containers = item.modItem as PotionofContainersTag;
    //        //        containers.chest = concoctionSlot.item.Clone();
    //        //        containers.chest.stack = 1;
    //        //        Main.LocalPlayer.QuickSpawnClonedItem(item);
    //        //        CheckSlowmode();
    //        //        ConsumeSlots();
    //        //        return true;
    //        //    }
    //        //    else
    //        //    {
    //        //        return false;
    //        //    }
    //        //}
    //        var item = ConcoctionDatabase.RecipeData[concoctionSlot.item.type].GetItem(potionSlot.item, concoctionSlot.item);
    //        if (item != null)
    //        {
    //            Main.LocalPlayer.QuickSpawnClonedItem(null, item);
    //            CheckSlowmode();
    //            ConsumeSlots();
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    public override void Update(GameTime gameTime)
    //    {
    //        base.Update(gameTime);
    //        Active = true;
    //        if (concocting == 0)
    //        {
    //            Aequus.NPCTalkInterface.SetState(null);
    //        }
    //        else
    //        {
    //            if (concocting > 0)
    //            {
    //                concocting--;
    //                if (concocting == 0)
    //                {
    //                    concocting = Math.Max(12 - concoctingTime / (slowMode ? 16 : 8) * 2, 4);
    //                    concoctingMax = concocting;
    //                    if (!SpawnConcoction())
    //                    {
    //                        concoctingTime = 0;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                concoctingTime = 0;
    //                concoctingMax = 0;
    //                slowMode = false;
    //            }
    //            potionSlot.Update(gameTime);
    //            concoctionSlot.Update(gameTime);
    //        }
    //    }

    //    private void DrawCauldron(Vector2 position)
    //    {
    //        if (concocting > 0)
    //        {
    //            Main.instance.LoadTiles(TileID.CookingPots);
    //            var texture = TextureAssets.Tile[TileID.CookingPots].Value;
    //            var frame = new Rectangle(texture.Width / 2, (int)(Main.GameUpdateCount % 24 / 6) * 36, 16, 16);
    //            position.X -= 16f * 0.8f;
    //            position.Y -= 16f * 0.8f;
    //            for (int i = 0; i < 2; i++)
    //            {
    //                for (int j = 0; j < 2; j++)
    //                {
    //                    var realFrame = new Rectangle(frame.X + i * 18, frame.Y + j * 18, frame.Width, frame.Height);
    //                    Main.spriteBatch.Draw(texture, new Vector2(position.X + i * 16f * 0.8f, position.Y + j * 16f * 0.8f), realFrame, new Color(255, 255, 255, 255), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            var texture = TextureAssets.Item[ItemID.Cauldron].Value;
    //            Main.spriteBatch.Draw(texture, new Vector2(position.X, position.Y), null, new Color(255, 255, 255, 255), 0f, texture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
    //        }
    //    }
    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        base.DrawSelf(spriteBatch);
    //        var player = Main.LocalPlayer;
    //        Main.hidePlayerCraftingMenu = true;
    //        var invBackTexture = TextureAssets.InventoryBack5.Value;
    //        int invBGWidth = (int)(invBackTexture.Width * 0.8f);
    //        int invBGHeight = (int)(invBackTexture.Height * 0.8f);
    //        var potionDimensions = potionSlot.GetDimensions();
    //        var concotionDimensions = potionSlot.GetDimensions();
    //        Main.spriteBatch.Draw(invBackTexture, potionDimensions.Position(), null, new Color(255, 255, 255, 255), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
    //        Main.spriteBatch.Draw(invBackTexture, concotionDimensions.Position(), null, new Color(255, 255, 255, 255), 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

    //        int textX = (int)((potionDimensions.X + concotionDimensions.X) / 2 + (int)(invBackTexture.Width * 0.8f) / 2);
    //        int textY = (int)(potionDimensions.Y + (int)(invBackTexture.Height * 0.8f) + 8);

    //        DrawCauldron(new Vector2(textX, potionDimensions.Y + invBackTexture.Height * 0.8f / 2));

    //        if (potionSlot.item != null && !potionSlot.item.IsAir)
    //        {
    //            float oldScale = Main.inventoryScale;
    //            Main.inventoryScale = 0.8f;
    //            ItemSlotRenderer.Draw(Main.spriteBatch, potionSlot.item, potionDimensions.Position());
    //            Main.inventoryScale = oldScale;
    //        }
    //        else
    //        {
    //            Main.instance.LoadItem(ItemID.PotionStatue);
    //            var bgItem = TextureAssets.Item[ItemID.PotionStatue].Value;
    //            Main.spriteBatch.Draw(bgItem, new Vector2(potionDimensions.X + invBGWidth / 2f, potionDimensions.Y + invBGHeight / 2f), null, new Color(100, 100, 100, 100), 0f, bgItem.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
    //        }
    //        if (concoctionSlot.item != null && !concoctionSlot.item.IsAir)
    //        {
    //            float oldScale = Main.inventoryScale;
    //            Main.inventoryScale = 0.8f;
    //            ItemSlotRenderer.Draw(Main.spriteBatch, concoctionSlot.item, concotionDimensions.Position());
    //            Main.inventoryScale = oldScale;
    //        }
    //        else
    //        {
    //            Main.instance.LoadItem(ItemID.StarStatue);
    //            var bgItem = TextureAssets.Item[ItemID.StarStatue].Value;
    //            Main.spriteBatch.Draw(bgItem, new Vector2(concotionDimensions.X + invBGWidth / 2f, concotionDimensions.Y + invBGHeight / 2f), null, new Color(100, 100, 100, 100), 0f, bgItem.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
    //        }

    //        bool playedConcoctTick = false;

    //        string text = concocting > 0 ? AequusText.GetText("StopConcocting") : AequusText.GetText("Concoct");
    //        var font = FontAssets.MouseText.Value;
    //        var measurement = font.MeasureString(text);

    //        textY += (int)measurement.Y;

    //        var textColor = new Color(255, 255, 255, 255);
    //        var textScale = Vector2.One;

    //        if (concocting <= 0 && Main.mouseX > potionDimensions.X && Main.mouseX < potionDimensions.X + invBackTexture.Width * 0.8f && Main.mouseY > potionDimensions.Y && Main.mouseY < potionDimensions.Y + invBackTexture.Height * 0.8f)
    //        {
    //            player.mouseInterface = true;
    //            if (Main.mouseLeft && Main.mouseLeftRelease && PotionSwap(potionSlot.item, Main.mouseItem))
    //            {
    //                Utils.Swap(ref potionSlot.item, ref Main.mouseItem);
    //                SoundEngine.PlaySound(SoundID.Grab);
    //            }
    //            if (potionSlot.item != null && !potionSlot.item.IsAir)
    //            {
    //                AequusUI.HoverItem(potionSlot.item);
    //            }
    //        }
    //        else if (concocting <= 0 && Main.mouseX > concotionDimensions.X && Main.mouseX < concotionDimensions.X + invBackTexture.Width * 0.8f && Main.mouseY > concotionDimensions.Y && Main.mouseY < concotionDimensions.Y + invBackTexture.Height * 0.8f)
    //        {
    //            player.mouseInterface = true;
    //            if (Main.mouseLeft && Main.mouseLeftRelease && ConcoctionSwap(concoctionSlot.item, Main.mouseItem))
    //            {
    //                Utils.Swap(ref concoctionSlot.item, ref Main.mouseItem);
    //                SoundEngine.PlaySound(SoundID.Grab);
    //            }
    //            if (concoctionSlot.item != null && !concoctionSlot.item.IsAir)
    //            {
    //                AequusUI.HoverItem(concoctionSlot.item);
    //            }
    //        }
    //        else
    //        {
    //            var hoverBox = Utils.CenteredRectangle(new Vector2(textX, textY), measurement);

    //            if (hoverBox.Contains(Main.mouseX, Main.mouseY))
    //            {
    //                player.mouseInterface = true;
    //                textColor = Color.Yellow;
    //                textScale *= 1.1f;
    //                if (!concoctTick)
    //                {
    //                    SoundEngine.PlaySound(SoundID.MenuTick);
    //                }
    //                playedConcoctTick = true;
    //                if (Main.mouseLeft && Main.mouseLeftRelease)
    //                {
    //                    if (!potionSlot.item.IsAir && !concoctionSlot.item.IsAir)
    //                    {
    //                        if (concocting > 0)
    //                        {
    //                            SoundEngine.PlaySound(SoundID.Item87);
    //                            concocting = 0;
    //                        }
    //                        else
    //                        {
    //                            SoundEngine.PlaySound(Aequus.GetSound("concoction" + Main.rand.Next(2)));
    //                            if (potionSlot.item.stack == 1 || concoctionSlot.item.stack == 1)
    //                            {
    //                                concocting = 1;
    //                            }
    //                            else
    //                            {
    //                                concocting = 30;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(textX, textY), textColor, 0f, measurement / 2f, textScale);

    //        concoctTick = playedConcoctTick;
    //    }

    //    private static bool PotionSwap(Item slotItem, Item mouseItem)
    //    {
    //        if (mouseItem != null && !mouseItem.IsAir &&
    //            ConcoctionDatabase.ConcoctiblePotion(mouseItem))
    //            return true;
    //        if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
    //            return true;
    //        return false;
    //    }
    //    private static bool ConcoctionSwap(Item slotItem, Item mouseItem)
    //    {
    //        if (mouseItem != null && !mouseItem.IsAir &&
    //            ConcoctionDatabase.RecipeData.ContainsKey(mouseItem.type))
    //            return true;
    //        if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
    //            return true;
    //        return false;
    //    }
    //}
}