using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor {
    public class KryptonPrefix : MossArmorPrefixBase {
        public override int MossItem => ItemID.KryptonMoss;

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            return new[] {
                new TooltipLine(Mod, "KryptonPrefixEffect", $"+{item.defense * 4} health from potions")
                { IsModifier = true, IsModifierBad = false, },

                new TooltipLine(Mod, "KryptonPrefixEffect2", $"+{item.defense} seconds of potion sickness")
                { IsModifier = true, IsModifierBad = true, }
            };
        }

        public override void Apply(Item item) {
        }

        public override void UpdateEquip(Item item, Player player) {
            player.potionDelayTime += item.defense;
            player.Aequus().extraHealingPotion += item.defense * 4;
        }
    }
}