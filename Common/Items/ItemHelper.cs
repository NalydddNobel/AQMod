﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items {
    public static class ItemHelper {
        internal static readonly string[] VanillaTooltipNames = new string[]
        {
            "ItemName",
            "Favorite", "FavoriteDesc",
            "Social", "SocialDesc",
            "Damage", "CritChance", "Speed", "Knockback",
            "FishingPower", "NeedsBait", "BaitPower",
            "Equipable",
            "WandConsumes",
            "Quest",
            "Vanity",
            "Defense",
            "PickPower", "AxePower", "HammerPower",
            "TileBoost",
            "HealLife", "HealMana", "UseMana",
            "Placeable",
            "Ammo",
            "Consumable",
            "Material",
            "Tooltip#",
            "EtherianManaWarning",
            "WellFedExpert", "BuffTime",
            "OneDropLogo",
            "PrefixDamage", "PrefixSpeed", "PrefixCritChance", "PrefixUseMana", "PrefixSize", "PrefixShootSpeed", "PrefixKnockback",
            "PrefixAccDefense", "PrefixAccMaxMana", "PrefixAccCritChance", "PrefixAccDamage", "PrefixAccMoveSpeed", "PrefixAccMeleeSpeed",
            "SetBonus",
            "Expert", "Master",
            "JourneyResearch",
            "BestiaryNotes",
            "SpecialPrice", "Price",
        };

        private static int FindLineIndex(string name) {
            if (string.IsNullOrEmpty(name)) {
                return 0;
            }

            if (name.StartsWith("Tooltip")) {
                name = "Tooltip#";
            }
            for (int i = 0; i < VanillaTooltipNames.Length; i++) {
                if (name == VanillaTooltipNames[i]) {
                    return i;
                }
            }
            return -1;
        }

        public static void AddTooltip(this List<TooltipLine> tooltips, TooltipLine line) {
            tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), line);
        }

        public static int GetIndex(this List<TooltipLine> tooltips, string lineName, int resultOffset = 0) {
            int myIndex = FindLineIndex(lineName);
            int i = 0;
            for (; i < tooltips.Count; i++) {
                if (tooltips[i].Mod == "Terraria" && FindLineIndex(tooltips[i].Name) >= myIndex) {
                    if (lineName == "Tooltip#") {
                        for (; i < tooltips.Count; i++) {
                            if (!tooltips[i].Name.StartsWith("Tooltip")) {
                                goto ReturnClamp;
                            }
                        }
                    }
                    goto ReturnClamp;
                }
            }
        ReturnClamp:
            return Math.Clamp(i + resultOffset, 0, tooltips.Count);
        }

        public static TooltipLine Find(this List<TooltipLine> tooltips, string name) {
            return tooltips.Find((t) => t != null && t.Mod != null && t.Name != null && t.Mod == "Terraria" && t.Name.Equals(name));
        }

        public static TooltipLine ItemName(this List<TooltipLine> tooltips) {
            return tooltips.Find("ItemName");
        }

        public static void RemoveKnockback(this List<TooltipLine> tooltips) {
            tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Knockback");
        }

        public static void RemoveCritChance(this List<TooltipLine> tooltips) {
            tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "CritChance");
        }

        public static void RemoveCritChanceModifier(this List<TooltipLine> tooltips) {
            tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "PrefixCritChance");
        }

        public static bool CanBeCraftedInto(int item, int craftedInto) {
            foreach (var r in Main.recipe) {
                if (r != null && r.createItem.type == craftedInto) {
                    foreach (var i in r.requiredItem) {
                        if (i.type == item)
                            return true;
                    }
                }
            }
            return false;
        }

        public static Item FindIngredient(this Recipe recipe, int itemID) {
            return recipe.requiredItem.Find((item) => item != null && !item.IsAir && item.type == itemID);
        }
        public static bool TryFindIngredient(this Recipe recipe, int itemID, out Item result) {
            result = recipe.FindIngredient(itemID);
            return result != null;
        }

        public static Recipe ReplaceItem(this Recipe r, int item, int newItem, int newItemStack = -1) {
            for (int i = 0; i < r.requiredItem.Count; i++) {
                if (r.requiredItem[i].type == item) {
                    int stack = newItemStack <= 0 ? r.requiredItem[i].stack : newItemStack;
                    r.requiredItem[i].SetDefaults(newItem);
                    r.requiredItem[i].stack = stack;
                    break;
                }
            }
            return r;
        }
        public static void ReplaceItemWith(this Recipe r, int item, Action<Recipe, Item> replacementMethod) {
            var itemList = new List<Item>(r.requiredItem);
            r.requiredItem.Clear();
            for (int i = 0; i < itemList.Count; i++) {
                if (itemList[i].type == item) {
                    replacementMethod(r, itemList[i]);
                }
                else {
                    r.AddIngredient(itemList[i].type, itemList[i].stack);
                }
            }
        }
    }
}