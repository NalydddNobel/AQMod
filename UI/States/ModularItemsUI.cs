using Aequus.Common.Catalogues;
using Aequus.Common.Utilities;
using Aequus.Items;
using Aequus.NPCs.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI.States
{
    public sealed class ModularItemsUI : UIState
    {
        public ItemSlotData itemSlot;
        public ItemSlotData[] moduleSlot;

        public int slotSpeed;
        public bool oldHasItem;

        private Texture2D InvBack => TextureAssets.InventoryBack3.Value;
        private float DrawScale => 0.8f;
        private int SlotHeight => (int)(InvBack.Height * DrawScale);

        public override void OnInitialize()
        {
            int x = UIHelper.LeftInventory(ignoreCreative: true) + 48;
            int y = UIHelper.BottomInventory;
            var back = InvBack;
            itemSlot = new ItemSlotData(x, y, back, new SpriteFrameData(TextureAssets.Extra[ExtrasID.EquipIcons], 3, 6, 1, 1, -2, -2)) { Scale = DrawScale, };
            moduleSlot = new ItemSlotData[ItemModuleTypeCatalogue.Count];
            int height = (int)(back.Height * DrawScale);
            for (int i = 0; i < moduleSlot.Length; i++)
            {
                moduleSlot[i] = new ItemSlotData(x, y, back) { Scale = DrawScale, };
                if (ItemModuleTypeCatalogue.TypeToTexture.ContainsKey(i))
                {
                    moduleSlot[i].icon = ItemModuleTypeCatalogue.TypeToTexture[i];
                }
            }
            Append(itemSlot);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            Main.LocalPlayer.QuickSpawnClonedItem(Main.LocalPlayer.GetSource_Misc("Aequus:ModularItemsUI"), itemSlot.item, itemSlot.item.stack);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<Exporter>())
            {
                Aequus.InventoryInterface.SetState(null);
                return;
            }
            for (int i = 0; i < moduleSlot.Length; i++)
            {
                moduleSlot[i].Update(gameTime);
            }
            bool hasItem = itemSlot.HasItem;
            if (hasItem != oldHasItem)
            {
                slotSpeed = 0;
            }
            else
            {
                slotSpeed = (int)MathHelper.Lerp(slotSpeed, 12f, 0.33f);
            }

            try
            {
                if (hasItem)
                {
                    int height = SlotHeight;
                    int[] validSlots = ModularItems.Catalogue.GetAllowedEquips(itemSlot.item.type);
                    int index = 0;
                    for (int i = moduleSlot.Length - 1; i >= 0; i--)
                    {
                        if (!validSlots.ContainsAny(i))
                        {
                            if (moduleSlot[i].Y < itemSlot.Y)
                            {
                                moduleSlot[i].Y = itemSlot.Y;
                            }
                            else if (moduleSlot[i].Y != itemSlot.Y)
                            {
                                moduleSlot[i].Y -= slotSpeed;
                            }
                            continue;
                        }
                        int gotoY = itemSlot.Y + height * (validSlots.Length - index);
                        if (moduleSlot[i].Y > gotoY)
                        {
                            moduleSlot[i].Y = gotoY;
                        }
                        else if (moduleSlot[i].Y < itemSlot.Y)
                        {
                            moduleSlot[i].Y = itemSlot.Y;
                        }
                        else if (moduleSlot[i].Y != gotoY)
                        {
                            moduleSlot[i].Y += slotSpeed;
                            if (moduleSlot[i].Y > gotoY)
                            {
                                moduleSlot[i].Y = gotoY;
                            }
                        }
                        else
                        {
                            moduleSlot[i].canHover = true;
                        }

                        index++;
                        if (i != 0 && moduleSlot[i].Y - itemSlot.Y < height)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < moduleSlot.Length; i++)
                    {
                        moduleSlot[i].canHover = false;
                        if (moduleSlot[i].Y < itemSlot.Y)
                        {
                            moduleSlot[i].Y = itemSlot.Y;
                        }
                        else if (moduleSlot[i].Y != itemSlot.Y)
                        {
                            moduleSlot[i].Y -= slotSpeed;
                        }
                    }
                }
            }
            catch
            {

            }

            oldHasItem = hasItem;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (itemSlot.GetHitbox().Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease && CanSwapGrapplingHook(itemSlot.item, Main.mouseItem))
                {
                    SoundEngine.PlaySound(SoundID.Grab);
                    Utils.Swap(ref itemSlot.item, ref Main.mouseItem);
                }
                if (!AequusHelpers.HasMouseItem && !itemSlot.item.IsAir)
                {
                    UIHelper.HoverItem(itemSlot.item, ItemSlot.Context.PrefixItem);
                }
            }

            DrawSlotBG(spriteBatch);

            float oldInvScale = Main.inventoryScale;
            Main.inventoryScale = DrawScale;
            itemSlot.Draw(spriteBatch);
            if (itemSlot.HasItem)
            {
                try
                {
                    var module = itemSlot.item.GetGlobalItem<ModularItems>();
                    var data = ModularItems.Catalogue.GetAllowedEquips(itemSlot.item.type);
                    for (int i = moduleSlot.Length - 1; i >= 0; i--)
                    {
                        if (moduleSlot[i].Y > itemSlot.Y)
                        {
                            if (module.modules != null && module.modules.dict.ContainsKey(i))
                            {
                                moduleSlot[i].item = module.modules.dict[i].Clone();
                            }
                            else
                            {
                                moduleSlot[i].item.TurnToAir();
                            }

                            if (moduleSlot[i].canHover && moduleSlot[i].GetHitbox().Contains(Main.mouseX, Main.mouseY))
                            {
                                Main.LocalPlayer.mouseInterface = true;
                                if (Main.mouseLeft && Main.mouseLeftRelease && CanSwapBarb(moduleSlot[i].item, Main.mouseItem))
                                {
                                    SoundEngine.PlaySound(SoundID.Grab);
                                    if (module.modules == null)
                                    {
                                        module.modules = new ModularItems.ItemModuleData();
                                    }
                                    if (Main.mouseItem == null || Main.mouseItem.IsAir)
                                    {
                                        module.modules.dict.Remove(i);
                                    }
                                    else
                                    {
                                        if (module.modules.dict.ContainsKey(i))
                                        {
                                            module.modules.dict[i] = Main.mouseItem;
                                        }
                                        else
                                        {
                                            module.modules.dict.Add(i, Main.mouseItem);
                                        }
                                    }
                                    Utils.Swap(ref moduleSlot[i].item, ref Main.mouseItem);
                                }
                                if (!AequusHelpers.HasMouseItem && !moduleSlot[i].item.IsAir)
                                {
                                    UIHelper.HoverItem(moduleSlot[i].item, ItemSlot.Context.PrefixItem);
                                }
                            }

                            moduleSlot[i].Draw(spriteBatch);
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                for (int i = moduleSlot.Length - 1; i >= 0; i--)
                {
                    if (moduleSlot[i].Y > itemSlot.Y)
                    {
                        moduleSlot[i].Draw(spriteBatch);
                    }
                }
            }
            base.Draw(spriteBatch);
            UIHelper.leftInvOffset += 60;
            Main.inventoryScale = oldInvScale;
        }
        private void DrawSlotBG(SpriteBatch spriteBatch)
        {
            int lowest = -1;
            int y = -1;
            for (int i = moduleSlot.Length - 1; i >= 0; i--)
            {
                if (moduleSlot[i].Y > y)
                {
                    lowest = i;
                    y = moduleSlot[i].Y;
                }
            }
            if (lowest != -1)
            {
                if (moduleSlot[lowest].Y != itemSlot.Y)
                {
                    var backgroundPosition = new Vector2(itemSlot.X, itemSlot.Y + 8 * DrawScale);
                    var backgroundScale = new Vector2(InvBack.Width * DrawScale, (moduleSlot[lowest].Y + (InvBack.Height - 8) * DrawScale) - backgroundPosition.Y);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, backgroundPosition, new Rectangle(0, 0, 1, 1), Color.Black, 0f, Vector2.Zero, backgroundScale, SpriteEffects.None, 0f);
                }
            }
        }

        public static bool CanSwapGrapplingHook(Item item, Item mouseItem)
        {
            if (mouseItem == null)
            {
                mouseItem = new Item();
            }
            if (mouseItem.IsAir && item.IsAir)
            {
                return false;
            }
            return mouseItem.IsAir || (!mouseItem.IsAir && mouseItem.stack == 1 && IsGrapplingHook(mouseItem));
        }
        public static bool IsGrapplingHook(Item item)
        {
            return ModularItems.Catalogue.CanEquipAnyModules(item.type);
        }
        public static bool CanSwapBarb(Item item, Item mouseItem)
        {
            if (mouseItem == null)
            {
                mouseItem = new Item();
            }
            if (mouseItem.IsAir && item.IsAir)
            {
                return false;
            }
            return mouseItem.IsAir || (!mouseItem.IsAir && mouseItem.stack == 1 && IsBarb(mouseItem));
        }
        public static bool IsBarb(Item item)
        {
            return ModularItems.Catalogue.ContainsModuleDataFor(item.type);
        }
    }
}