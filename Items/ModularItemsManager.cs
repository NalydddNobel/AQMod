using Aequus.Common.ID;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Items
{
    public sealed class ModularItemsManager : GlobalItem
    {
        public sealed class Catalogue
        {
            private static Dictionary<int, IItemModuleData> _registeredModules;
            private static Dictionary<int, List<int>> _allowedEquips;

            public static void AllowEquipType(int item, List<int> equipTypes)
            {
                if (_allowedEquips.ContainsKey(item))
                {
                    _allowedEquips[item].AddRange(equipTypes);
                }
                else
                {
                    _allowedEquips.Add(item, equipTypes);
                }
            }
            public static void AllowEquipType(int item, int equipType)
            {
                AllowEquipType(item, new List<int>() { equipType, });
            }
            public static void RemoveEquipType(int item, List<int> equipTypes)
            {
                if (_allowedEquips.ContainsKey(item))
                {
                    foreach (var type in equipTypes)
                    {
                        _allowedEquips[item].Remove(type);
                    }
                }
            }
            public static void RemoveEquipType(int item, int equipType)
            {
                RemoveEquipType(item, new List<int>() { equipType, });
            }
            public static void DefaultToHookEquipDamageDebuffs(int item)
            {
                AllowEquipType(item, new List<int>()
                {
                    ItemModuleType.BarbDamaging,
                    ItemModuleType.BarbDebuff,
                });
            }
            public static void DefaultToAllAequusHookEquips(int item)
            {
                AllowEquipType(item, new List<int>()
                {
                    ItemModuleType.BarbDamaging,
                    ItemModuleType.BarbDebuff,
                    ItemModuleType.BarbMovementOverhaul,
                    ItemModuleType.BarbMeathook,
                });
            }

            public static void RegisterModule(int item, IItemModuleData data)
            {
                _registeredModules.Add(item, data);
            }

            public static IItemModuleData GetModuleData(int item)
            {
                return _registeredModules[item];
            }
            public static bool TryGetModuleData(int item, out IItemModuleData value)
            {
                return _registeredModules.TryGetValue(item, out value);
            }
            public static int[] GetAllowedEquips(int item)
            {
                return _allowedEquips[item].ToArray();
            }
            public static bool TryGetAllowedModules(int item, out int[] value)
            {
                if (_allowedEquips.TryGetValue(item, out var list))
                {
                    value = list.ToArray();
                    return true;
                }
                value = default(int[]);
                return false;
            }

            internal static void Load()
            {
                _registeredModules = new Dictionary<int, IItemModuleData>();
                // Hook of Dissonance and other teleporting hooks should not be allowed to use barbs
                // Anti Gravity Hook and Static hook are also purposely not allowed to use barbs
                _allowedEquips = new Dictionary<int, List<int>>();
                DefaultToAllAequusHookEquips(ItemID.GrapplingHook);
                DefaultToAllAequusHookEquips(ItemID.IvyWhip);
                DefaultToAllAequusHookEquips(ItemID.SkeletronHand);
                DefaultToAllAequusHookEquips(ItemID.SlimeHook);
                DefaultToAllAequusHookEquips(ItemID.BatHook);
                DefaultToAllAequusHookEquips(ItemID.CandyCaneHook);
                DefaultToAllAequusHookEquips(ItemID.DualHook);
                DefaultToAllAequusHookEquips(ItemID.ThornHook);
                DefaultToAllAequusHookEquips(ItemID.IlluminantHook);
                DefaultToAllAequusHookEquips(ItemID.WormHook);
                DefaultToAllAequusHookEquips(ItemID.TendonHook);
                DefaultToAllAequusHookEquips(ItemID.SpookyHook);
                DefaultToAllAequusHookEquips(ItemID.ChristmasHook);
                DefaultToAllAequusHookEquips(ItemID.LunarHook);

                DefaultToHookEquipDamageDebuffs(ItemID.SquirrelHook);
                DefaultToHookEquipDamageDebuffs(ItemID.WebSlinger);
                DefaultToHookEquipDamageDebuffs(ItemID.AmethystHook);
                DefaultToHookEquipDamageDebuffs(ItemID.TopazHook);
                DefaultToHookEquipDamageDebuffs(ItemID.SapphireHook);
                DefaultToHookEquipDamageDebuffs(ItemID.EmeraldHook);
                DefaultToHookEquipDamageDebuffs(ItemID.RubyHook);
                DefaultToHookEquipDamageDebuffs(ItemID.DiamondHook);
            }

            internal static void Unload()
            {
                _allowedEquips?.Clear();
                _allowedEquips = null;
            }
        }

        public interface IItemModuleData
        {
            List<int> ModuleTypes { get; set; }
        }

        public Item[] moduleItems;

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            var clone = (ModularItemsManager)base.Clone(item, itemClone);
            clone.moduleItems = moduleItems;
            return clone;
        }

        public override void Load()
        {
            Catalogue.Load();
        }

        public override void Unload()
        {
            Catalogue.Unload();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
            {
                if (Catalogue.TryGetAllowedModules(item.type, out var equips) && tooltips != null)
                {
                    string spillage = "";
                    //foreach (var e in equips)
                    //{
                    //    if (spillage == "")
                    //    {
                    //        spillage += e;
                    //    }
                    //    else
                    //    {
                    //        spillage += ", " + e;
                    //    }
                    //}
                    //tooltips.Add(new TooltipLine(Mod, "BarbCheck", "This hook barb can equip these types:\n" + spillage));

                    if (moduleItems != null)
                    {
                        spillage = "";
                        foreach (var e in moduleItems)
                        {
                            if (spillage == "")
                            {
                                spillage += "[i:" + e.type + "]";
                            }
                            else
                            {
                                spillage += ", [i: " + e.type + "]";
                            }
                        }
                        tooltips.Add(new TooltipLine(Mod, "BarbCheck", "Equipped modules: \n" + spillage));
                    }
                }
            }
            catch
            {
            }
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Catalogue.TryGetAllowedModules(item.type, out var value) && UISystem.ValidModularSlotContext.Contains(UISystem.ItemSlotContext))
            {
                var back = position + frame.Size() / 2f * scale - TextureAssets.InventoryBack.Value.Size() / 2f * Main.inventoryScale;
                var backHitbox = new Rectangle((int)back.X, (int)back.Y, (int)(TextureAssets.InventoryBack.Value.Width * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Value.Height * Main.inventoryScale));
                if (backHitbox.Contains(Main.mouseX, Main.mouseY))
                {
                    if (Main.mouseItem != null && !Main.mouseItem.IsAir && Catalogue.TryGetModuleData(Main.mouseItem.type, out var value2))
                    {
                        if (moduleItems != null)
                        {
                            foreach (var i in moduleItems)
                            {
                                var value3 = Catalogue.GetModuleData(i.type);
                                if (value2.ModuleTypes.ContainsAny(value3.ModuleTypes))
                                {
                                    return;
                                }
                            }
                        }
                        UISystem.DisableItemLeftClick = 2;
                        Main.HoverItem = new Item();
                        Main.hoverItemName = "";
                        Main.instance.MouseText(Language.GetTextValue(ApplyModuleHoverText(item, Main.mouseItem)), 2);
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Main.mouseLeftRelease = false;
                            AddModule(Main.mouseItem.Clone());
                            Main.mouseItem.TurnToAir();
                        }
                    }
                    else
                    {
                        if (moduleItems != null)
                        {
                            bool unequip = false;
                            if (ItemSlot.NotUsingGamepad)
                            {
                                unequip = (ItemSlot.Options.DisableLeftShiftTrashCan && ItemSlot.ShiftInUse) || (!ItemSlot.Options.DisableLeftShiftTrashCan && ItemSlot.ControlInUse);
                            }
                            else
                            {
                                unequip = ItemSlot.ControlInUse;
                            }
                            if (unequip)
                            {
                                Main.cursorOverride = 8;
                                UISystem.DisableItemLeftClick = 2;
                                Main.HoverItem = new Item();
                                Main.hoverItemName = "";
                                Main.instance.MouseText(Language.GetTextValue(UnequipModuleHoverText(item, moduleItems)), 2);
                                if (Main.mouseLeft && Main.mouseLeftRelease)
                                {
                                    Main.mouseLeftRelease = false;
                                    Main.mouseItem = moduleItems[0].Clone();
                                    for (int i = 1; i < moduleItems.Length; i++)
                                    {
                                        moduleItems[i - 1] = moduleItems[i];
                                    }
                                    Array.Resize(ref moduleItems, moduleItems.Length - 1);
                                    if (moduleItems.Length == 0)
                                    {
                                        moduleItems = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private string UnequipModuleHoverText(Item item, Item[] modules)
        {
            return "Unequip module?";
        }
        private string ApplyModuleHoverText(Item item, Item mouseItem)
        {
            return "Apply module?";
        }
        public void AddModule(Item module)
        {
            if (moduleItems == null)
            {
                moduleItems = new Item[] { module };
                return;
            }
            Array.Resize(ref moduleItems, moduleItems.Length + 1);
            moduleItems[^1] = module;
        }
    }
}