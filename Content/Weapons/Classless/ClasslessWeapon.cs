using System.Collections.Generic;
using Terraria.Utilities;

namespace Aequus.Content.Weapons.Classless;

public abstract class ClasslessWeapon : ModItem {
    // Poopy
    public override int ChoosePrefix(UnifiedRandom rand) {
        var prefixes = PrefixLoader.GetPrefixesInCategory(PrefixCategory.Custom);
        List<int> list = new() {
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
        };
        if (Item.knockBack > 0f) {
            list.Add(PrefixID.Weak);
            list.Add(PrefixID.Forceful);
            list.Add(PrefixID.Strong);
        }
        if (Item.mana > 0) {
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
        if (Item.shoot > ProjectileID.None && Item.shootSpeed > 0f) {
            list.Add(PrefixID.Unreal);
            list.Add(PrefixID.Deadly2);
            list.Add(PrefixID.Intimidating);
            list.Add(PrefixID.Rapid);
            list.Add(PrefixID.Hasty);
            list.Add(PrefixID.Awful);
            list.Add(PrefixID.Lethargic);
        }
        if (Item.DamageType.UseStandardCritCalcs) {
            list.Add(PrefixID.Keen);
            list.Add(PrefixID.Zealous);
        }
        foreach (var prefix in prefixes) {
            if (prefix.CanRoll(Item)) {
                list.Add(prefix.Type);
            }
        }
        return list.Count > 0 ? rand.Next(list) : 0;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (!Item.DamageType.UseStandardCritCalcs) {
            tooltips.RemoveCritChanceModifier();
        }
    }
}