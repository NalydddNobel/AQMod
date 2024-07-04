using AequusRemake.Core.Entities.Items.Components;
using System.Collections.Generic;
using Terraria.Utilities;

namespace AequusRemake.Core.Entities.Items;

public class ClasslessGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.ModItem is IOmniclassItem;
    }

    public override int ChoosePrefix(Item item, UnifiedRandom rand) {
        IReadOnlyList<ModPrefix> prefixes = PrefixLoader.GetPrefixesInCategory(PrefixCategory.Custom);

        // Generic prefixes all classless weapons should get.
        List<int> list = [
            PrefixID.Broken,
            PrefixID.Annoying,
            PrefixID.Damaged,
            PrefixID.Shoddy,
            PrefixID.Unpleasant,
            PrefixID.Hurtful,
            PrefixID.Ruthless,
            PrefixID.Superior,
            PrefixID.Demonic,
            PrefixID.Godly,
            PrefixID.Murderous,
            PrefixID.Nasty,
        ];

        // Generic prefixes which effect knockback.
        if (item.knockBack > 0f) {
            list.Add(PrefixID.Weak);
            list.Add(PrefixID.Forceful);
            list.Add(PrefixID.Strong);
        }

        // Generic magic prefixes for magical-classless items.
        if (item.mana > 0) {
            list.Add(PrefixID.Mystic);
            list.Add(PrefixID.Mythical);
            list.Add(PrefixID.Adept);
            list.Add(PrefixID.Masterful);
            list.Add(PrefixID.Inept);
            list.Add(PrefixID.Ignorant);
            list.Add(PrefixID.Intense);
            list.Add(PrefixID.Taboo);
            list.Add(PrefixID.Celestial);
            list.Add(PrefixID.Furious);
        }

        // Generic ranged prefixes for ranged-classless items.
        if (item.shoot > ProjectileID.None && item.shootSpeed > 0f) {
            list.Add(PrefixID.Unreal);
            list.Add(PrefixID.Deadly2);
            list.Add(PrefixID.Intimidating);
            list.Add(PrefixID.Rapid);
            list.Add(PrefixID.Hasty);
            list.Add(PrefixID.Awful);
            list.Add(PrefixID.Lethargic);
        }

        // Generic crit prefixes for classless items which can randomly critical strike.
        if (item.DamageType.UseStandardCritCalcs) {
            list.Add(PrefixID.Keen);
            list.Add(PrefixID.Zealous);
        }

        // Modded prefixes.
        foreach (ModPrefix prefix in prefixes) {
            if (prefix.CanRoll(item)) {
                list.Add(prefix.Type);
            }
        }

        return list.Count > 0 ? rand.Next(list) : -1;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (!item.DamageType.UseStandardCritCalcs) {
            tooltips.RemoveCritChanceModifier();
        }
    }
}
