using System.Collections.Generic;

namespace AequusRemake.Core.Util.Helpers;

public sealed class TooltipServices : ILoad {
    internal static readonly string[] VanillaTooltipNamesInOrder = [
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
    ];

    internal static Dictionary<string, int> NameToIndex;

    /// <summary>Attempts to get the index of the tooltip which matches <paramref name="name"/>.</summary>
    /// <param name="name">The name of the tooltip line.</param>
    /// <returns>-1 if there is no match.</returns>
    internal static int GetIndex(string name) {
        if (string.IsNullOrEmpty(name)) {
            return 0;
        }

        if (name.StartsWith("Tooltip")) {
            name = "Tooltip#";
        }

        return NameToIndex.TryGetValue(name, out int index) ? index : -1;
    }

    void ILoad.Load(Mod mod) {
        NameToIndex = VanillaTooltipNamesInOrder.AllocLookup(throwOnDuplicates: true);
    }

    void ILoad.Unload() { }
}
