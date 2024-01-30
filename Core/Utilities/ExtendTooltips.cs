using System;
using System.Collections.Generic;

namespace Aequus.Core.Utilities;

public static class ExtendTooltips {
    internal static readonly String[] VanillaTooltipNames = new[] {
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

    private static Int32 FindLineIndex(String name) {
        if (String.IsNullOrEmpty(name)) {
            return 0;
        }

        if (name.StartsWith("Tooltip")) {
            name = "Tooltip#";
        }
        for (Int32 i = 0; i < VanillaTooltipNames.Length; i++) {
            if (name == VanillaTooltipNames[i]) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>Inserts a new tooltip line to the end of the item's tooltip.</summary>
    public static void AddTooltip(this List<TooltipLine> tooltips, TooltipLine line) {
        tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), line);
    }

    /// <summary>
    /// Attempts to find a suitable index to insert a tooltip using a pre-baked list of tooltip line names.
    /// </summary>
    /// <param name="tooltips"></param>
    /// <param name="lineName">The line's name, make sure this matches one of the entries in <see cref="VanillaTooltipNames"/></param>
    /// <param name="resultOffset">A flat offset to the line result.</param>
    /// <returns>The best index to insert a new line into.</returns>
    public static Int32 GetIndex(this List<TooltipLine> tooltips, String lineName, Int32 resultOffset = 0) {
        Int32 myIndex = FindLineIndex(lineName);
        Int32 i = 0;
        for (; i < tooltips.Count; i++) {
            if (tooltips[i].Mod != "Terraria") {
                continue;
            }

            Int32 index = FindLineIndex(tooltips[i].Name);
            if (index >= myIndex) {
                if (index != myIndex) {
                    if (resultOffset > 0) {
                        resultOffset--;
                    }
                }
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

    /// <returns>The <see cref="TooltipLine"/> with a specified name. (And is marked as "Terraria")</returns>
    public static TooltipLine Find(this List<TooltipLine> tooltips, String name) {
        return tooltips.Find((t) => t != null && t.Mod.Equals("Terraria") && t.Name.Equals(name));
    }

    /// <returns>The <see cref="TooltipLine"/> for an Item's name.</returns>
    public static TooltipLine ItemName(this List<TooltipLine> tooltips) {
        return tooltips.Find("ItemName");
    }

    /// <summary>Removes the "Knockback" tooltip line, if it exists.</summary>
    public static void RemoveKnockback(this List<TooltipLine> tooltips) {
        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Knockback");
    }

    /// <summary>Removes the "CritChance" tooltip line, if it exists.</summary>
    public static void RemoveCritChance(this List<TooltipLine> tooltips) {
        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "CritChance");
    }

    /// <summary>Removes the "PrefixCritChance" tooltip line, if it exists.</summary>
    public static void RemoveCritChanceModifier(this List<TooltipLine> tooltips) {
        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "PrefixCritChance");
    }
}
