using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Chat;

namespace AequusRemake.Core.Utilities;

public static class ExtendTooltips {
    internal static readonly string[] VanillaTooltipNames = new[] {
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
        int myIndex = FindLineIndex(lineName);
        int i = 0;
        for (; i < tooltips.Count; i++) {
            if (tooltips[i].Mod != "Terraria") {
                continue;
            }

            int index = FindLineIndex(tooltips[i].Name);
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

    internal static void TestGrabBagTooltip(Item item, List<TooltipLine> tooltips) {
        List<string> dropTable = GetListOfDrops(Main.ItemDropsDB.GetRulesForItemID(item.type));

        for (int i = 0; i < dropTable.Count; i++) {
            tooltips.Add(new TooltipLine(AequusRemake.Instance, $"Drop{i}", dropTable[i]));
        }
    }

    private static List<string> GetListOfDrops(List<IItemDropRule> dropTable) {
        var tooltips = new List<string>();
        if (dropTable.Count == 0) {
            return tooltips;
        }

        foreach (var rule in dropTable) {
            var drops = new List<DropRateInfo>();
            rule.ReportDroprates(drops, new DropRateInfoChainFeed(1f));
            tooltips.Add(rule.GetType().FullName + ":");
            foreach (var drop in drops) {
                string text = "* " + ItemTagHandler.GenerateTag(new Item(drop.itemId));
                if (drop.stackMin == drop.stackMax) {
                    if (drop.stackMin > 1) {
                        text += $" ({drop.stackMin})";
                    }
                }
                else {
                    text += $" ({drop.stackMin} - {drop.stackMax})";
                }
                text += " " + (int)(drop.dropRate * 10000f) / 100f + "%";
                tooltips.Add(text);
                if (drop.conditions != null && drop.conditions.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftControl)) {
                    foreach (var cond in drop.conditions) {
                        if (cond == null) {
                            continue;
                        }

                        string extraDesc = cond.GetConditionDescription();
                        string condText = Main.keyState.IsKeyDown(Keys.LeftShift) ? cond.GetType().FullName : cond.GetType().Name;
                        if (!string.IsNullOrEmpty(extraDesc))
                            condText = $"{condText} '{extraDesc}': {cond.CanDrop(info: new DropAttemptInfo() { IsInSimulation = false, item = -1, npc = Main.npc[0], player = Main.LocalPlayer, rng = Main.rand, IsExpertMode = Main.expertMode, IsMasterMode = Main.masterMode })}";
                        tooltips.Add(condText);
                    }
                }
            }
        }
        return tooltips;
    }
}
