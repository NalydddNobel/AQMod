using System.Linq;
using Aequus.Common.Catalogues;
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
            int x = UIHelper.LeftInventory(ignoreCreative: true) + 60;
            int y = UIHelper.BottomInventory;
            var back = InvBack;
            itemSlot = new ItemSlotData(x, y, back);
            moduleSlot = new ItemSlotData[ItemModuleTypeCatalogue.Count];
            int height = (int)(back.Height * DrawScale);
            for (int i = 0; i < moduleSlot.Length; i++)
            {
                moduleSlot[i] = new ItemSlotData(x, y + height, back);
            }
            Append(itemSlot);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<Exporter>())
            {
                UIHelper.InventoryInterface.SetState(null);
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
                slotSpeed = (int)MathHelper.Lerp(slotSpeed, 24f, 0.33f);
            }

            try
            {
                if (hasItem)
                {
                    int height = SlotHeight;
                    int[] validSlots = ModularItemsManager.Catalogue.GetAllowedEquips(itemSlot.item.type);
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
            if (itemSlot.GetHitbox(DrawScale).Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease && CanSwapGrapplingHook(itemSlot.item, Main.mouseItem))
                {
                    SoundEngine.PlaySound(SoundID.Grab);
                    Utils.Swap(ref itemSlot.item, ref Main.mouseItem);
                }
                if (!AequusHelpers.HasMouseItem() && !itemSlot.item.IsAir)
                {
                    UIHelper.HoverItem(itemSlot.item, ItemSlot.Context.PrefixItem);
                }
            }

            float oldInvScale = Main.inventoryScale;
            Main.inventoryScale = DrawScale;
            itemSlot.Draw(spriteBatch);
            if (itemSlot.HasItem)
            {
                try
                {
                    var module = itemSlot.item.GetGlobalItem<ModularItemsManager>();
                    var data = ModularItemsManager.Catalogue.GetAllowedEquips(itemSlot.item.type);
                    for (int i = moduleSlot.Length - 1; i >= 0; i--)
                    {
                        if (moduleSlot[i].Y > itemSlot.Y)
                        {
                            if (module.modules != null && module.modules.modules.ContainsKey(i))
                            {
                                moduleSlot[i].item = module.modules.modules[i].Clone();
                            }
                            else
                            {
                                moduleSlot[i].item.TurnToAir();
                            }

                            if (moduleSlot[i].GetHitbox(DrawScale).Contains(Main.mouseX, Main.mouseY))
                            {
                                Main.LocalPlayer.mouseInterface = true;
                                if (Main.mouseLeft && Main.mouseLeftRelease && CanSwapBarb(moduleSlot[i].item, Main.mouseItem))
                                {
                                    SoundEngine.PlaySound(SoundID.Grab);
                                    if (module.modules == null)
                                    {
                                        module.modules = new ModularItemsManager.ModuleData();
                                    }
                                    if (Main.mouseItem == null || Main.mouseItem.IsAir)
                                    {
                                        module.modules.modules.Remove(i);
                                    }
                                    else
                                    {
                                        if (module.modules.modules.ContainsKey(i))
                                        {
                                            module.modules.modules[i] = Main.mouseItem;
                                        }
                                        else
                                        {
                                            module.modules.modules.Add(i, Main.mouseItem);
                                        }
                                    }
                                    Utils.Swap(ref moduleSlot[i].item, ref Main.mouseItem);
                                }
                                if (!AequusHelpers.HasMouseItem() && !moduleSlot[i].item.IsAir)
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
            base.Draw(spriteBatch);
            UIHelper.leftInvOffset += 60;
            Main.inventoryScale = oldInvScale;
        }
        public static bool CanSwapGrapplingHook(Item item, Item mouseItem)
        {
            if (mouseItem == null)
            {
                mouseItem = new Item();
            }
            if (item.IsAir)
            {
                return IsGrapplingHook(mouseItem);
            }
            if (!mouseItem.IsAir)
            {
                return IsGrapplingHook(item);
            }
            return true;
        }
        public static bool CanSwapBarb(Item item, Item mouseItem)
        {
            if (mouseItem == null)
            {
                mouseItem = new Item();
            }
            if (item.IsAir)
            {
                return IsBarb(mouseItem);
            }
            if (!mouseItem.IsAir)
            {
                return IsBarb(item);
            }
            return true;
        }
        public static bool IsGrapplingHook(Item item)
        {
            return ModularItemsManager.Catalogue.CanEquipAnyModules(item.type);
        }
        public static bool IsBarb(Item item)
        {
            return ModularItemsManager.Catalogue.ContainsModuleDataFor(item.type);
        }
    }
}