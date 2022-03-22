using AQMod.Items.Potions.Special;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.HookLists
{
    public sealed class ItemHooklist : HookList
    {
        [LoadHook(typeof(Item), "SetDefaults", BindingFlags.Public | BindingFlags.Instance)]
        internal static void PostSetDefaults(On.Terraria.Item.orig_SetDefaults orig, Item self, int Type, bool noMatCheck)
        {
            orig(self, Type, noMatCheck);
            try
            {
                if (!AQMod.Loading)
                {
                    self.GetGlobalItem<AQItem>().PostSetDefaults(self, Type, noMatCheck);
                }
            }
            catch
            {
            }
        }

        #region Split Hooks

        [LoadHook("Split", "Split.Items.Misc.AlchemicalPot", "UpdateGradient", BindingFlags.NonPublic | BindingFlags.Instance)]
        internal static void Split_AlchemistPot_UpdateGradient(Action<ModItem> orig, ModItem self)
        {
            orig(self);
            if (!Main.dedServ)
            {
                var list = (List<Item>)self.GetType().GetField("potions", BindingFlags.Public | BindingFlags.Instance).GetValue(self);
                var colors = new Color[list.Count];
                var drawHelper = AQMod.split.mod.GetType().GetField("DrawHelper").GetValue(AQMod.split.mod);
                var getItemColor = drawHelper.GetType().GetMethod("GetPotionColor", BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < colors.Length; i++)
                {
                    if (list[i]?.modItem is ConcoctionResult concoction)
                    {
                        colors[i] = (Color)getItemColor.Invoke(drawHelper, new object[] { concoction.original.type });
                    }
                    else
                    {
                        colors[i] = (Color)getItemColor.Invoke(drawHelper, new object[] { list[i].type });
                    }
                }

                //AQMod.Instance.Logger.Debug("1");
                var f = self.GetType().GetField("potionGradient", BindingFlags.Public | BindingFlags.Instance);
                //AQMod.Instance.Logger.Debug("2, f is {" + (f == null ? "null" : f.Name) + "}");
                var type = AQMod.split.mod.Code.GetType("Split.Helpers.Gradient");
                //AQMod.Instance.Logger.Debug("3, type is {" + (type == null ? "null" : type.Name) + "}");
                var method = type.GetMethod("Linear");
                //AQMod.Instance.Logger.Debug("4, type is {" + (method == null ? "null" : method.Name) + "}");
                f.SetValue(self, method.Invoke(null, new object[] { true, colors, }));
            }
        }

        [LoadHook("Split", "Split.Items.Misc.AlchemicalPot", "AddPotion", BindingFlags.NonPublic | BindingFlags.Instance)]
        internal static void Split_AlchemistPot_AddPotion(Action<ModItem, Item> orig, ModItem self, Item item)
        {
            if (item?.modItem is ConcoctionResult)
            {
                var f = self.GetType().GetField("potions", BindingFlags.Public | BindingFlags.Instance);
                var list = (List<Item>)f.GetValue(self);
                foreach (var i in list)
                {
                    if (i.buffType == item.buffType)
                    {
                        if (i.type == item.type)
                        {
                            i.stack++;
                            item.stack--;
                            if (item.stack < 0)
                            {
                                item.TurnToAir();
                            }
                        }
                        return;
                    }
                }
                list.Add(item.Clone());
                f.SetValue(self, list);
                item.stack = 0;
                item.TurnToAir();
                return;
            }
            orig(self, item);
        }

        #endregion
    }
}