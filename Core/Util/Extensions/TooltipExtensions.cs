using AequusRemake.Core.Util.Helpers;
using System;
using System.Collections.Generic;

namespace AequusRemake.Core.Util.Extensions;

public static class TooltipExtensions {
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
    public static int GetIndex(this List<TooltipLine> tooltips, string lineName, int resultOffset = 0) {
        int myIndex = TooltipServices.GetIndex(lineName);
        int i = 0;
        for (; i < tooltips.Count; i++) {
            if (tooltips[i].Mod != "Terraria") {
                continue;
            }

            int index = TooltipServices.GetIndex(tooltips[i].Name);
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
    public static TooltipLine Find(this List<TooltipLine> tooltips, string name) {
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
