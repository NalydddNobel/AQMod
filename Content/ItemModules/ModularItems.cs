using Aequus.Common.Catalogues;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.ItemModules
{
    public class ModularItems : GlobalItem
    {
        public sealed class Catalogue
        {
            private static Dictionary<int, IItemModule> _registeredModules;
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
            public static void DefaultHookOnlyBarbHookEquips(int item)
            {
                AllowEquipType(item, new List<int>()
                {
                    ItemModuleTypes.BarbHook,
                });
            }
            public static void DefaultAllBarbHookEquips(int item)
            {
                AllowEquipType(item, new List<int>()
                {
                    ItemModuleTypes.BarbHook,
                    ItemModuleTypes.BarbChain,
                    ItemModuleTypes.BarbMisc,
                });
            }

            public static void RegisterModule(int item, IItemModule data)
            {
                _registeredModules.Add(item, data);
            }

            public static bool ContainsModuleDataFor(int item)
            {
                return _registeredModules.ContainsKey(item);
            }

            public static IItemModule GetModuleData(int item)
            {
                return _registeredModules[item];
            }
            public static bool TryGetModuleData(int item, out IItemModule value)
            {
                return _registeredModules.TryGetValue(item, out value);
            }
            public static bool CanEquipAnyModules(int item)
            {
                return _allowedEquips.ContainsKey(item);
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
                _registeredModules = new Dictionary<int, IItemModule>();
                // Hook of Dissonance and other teleporting hooks should not be allowed to use barbs
                // Anti Gravity Hook and Static hook are also purposely not allowed to use barbs
                _allowedEquips = new Dictionary<int, List<int>>();
                DefaultAllBarbHookEquips(ItemID.GrapplingHook);
                DefaultAllBarbHookEquips(ItemID.IvyWhip);
                DefaultAllBarbHookEquips(ItemID.SkeletronHand);
                DefaultAllBarbHookEquips(ItemID.SlimeHook);
                DefaultAllBarbHookEquips(ItemID.BatHook);
                DefaultAllBarbHookEquips(ItemID.CandyCaneHook);
                DefaultAllBarbHookEquips(ItemID.DualHook);
                DefaultAllBarbHookEquips(ItemID.ThornHook);
                DefaultAllBarbHookEquips(ItemID.IlluminantHook);
                DefaultAllBarbHookEquips(ItemID.WormHook);
                DefaultAllBarbHookEquips(ItemID.TendonHook);
                DefaultAllBarbHookEquips(ItemID.SpookyHook);
                DefaultAllBarbHookEquips(ItemID.ChristmasHook);
                DefaultAllBarbHookEquips(ItemID.LunarHook);

                DefaultHookOnlyBarbHookEquips(ItemID.SquirrelHook);
                DefaultHookOnlyBarbHookEquips(ItemID.WebSlinger);
                DefaultHookOnlyBarbHookEquips(ItemID.AmethystHook);
                DefaultHookOnlyBarbHookEquips(ItemID.TopazHook);
                DefaultHookOnlyBarbHookEquips(ItemID.SapphireHook);
                DefaultHookOnlyBarbHookEquips(ItemID.EmeraldHook);
                DefaultHookOnlyBarbHookEquips(ItemID.RubyHook);
                DefaultHookOnlyBarbHookEquips(ItemID.DiamondHook);
            }

            internal static void Unload()
            {
                _allowedEquips?.Clear();
                _allowedEquips = null;
            }
        }

        public ItemModuleData itemModules;

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            var clone = (ModularItems)base.Clone(item, itemClone);
            clone.itemModules = itemModules;
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

        public override void SaveData(Item item, TagCompound tag)
        {
            if (itemModules != null)
            {
                tag["modules"] = itemModules.SerializeData();
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("modules"))
            {
                try
                {
                    itemModules = ItemModuleData.LoadData(tag.Get<TagCompound>("modules"));
                }
                catch (Exception ex)
                {
                    Aequus.Instance.Logger.Error(ex);
                    itemModules = null;
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
            {
                if (Catalogue.TryGetAllowedModules(item.type, out var equips) && tooltips != null)
                {
                    if (itemModules != null && itemModules.Count > 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            tooltips.Add(new TooltipLine(Mod, "ModularNone" + i, "-"));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus" && line.Name.StartsWith("ModularNone"))
            {
                return false;
            }
            return true;
        }

        public override void PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (itemModules != null && itemModules.Count > 0)
            {
                float X = lines[0].X;
                float y = 0f;

                foreach (var l in lines)
                {
                    if (l.Mod == "Aequus" && l.Name == "ModularNone0")
                    {
                        y += l.Y;
                    }
                }

                float oldScale = Main.inventoryScale;
                Main.inventoryScale = 1f;

                var back = TextureAssets.InventoryBack.Value;
                float backWidth = back.Width * Main.inventoryScale;
                int i = 0;
                foreach (var module in itemModules)
                {
                    float x = X + (backWidth + 4) * i;
                    Main.spriteBatch.Draw(back, new Vector2(x, y), null, new Color(255, 255, 255, 255), 0f, new Vector2(0f, 0f), Main.inventoryScale, SpriteEffects.None, 0f);

                    int itemType = module.Value.type;
                    Main.instance.LoadItem(itemType);
                    var itemTexture = TextureAssets.Item[itemType].Value;
                    var itemScale = 1f;
                    int max = itemTexture.Width > itemTexture.Height ? itemTexture.Width : itemTexture.Height;
                    if (max > 32)
                    {
                        itemScale = 32f / max;
                    }
                    Main.spriteBatch.Draw(itemTexture, new Vector2(x, y) + back.Size() / 2f, null, Color.White, 0f, itemTexture.Size() / 2f, Main.inventoryScale * itemScale, SpriteEffects.None, 0f);
                    i++;
                }

                Main.inventoryScale = oldScale;
            }
        }
    }
}