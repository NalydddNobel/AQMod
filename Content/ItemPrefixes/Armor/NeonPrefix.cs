using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor {
    public class NeonPrefix : MossArmorPrefixBase {
        public override int MossItem => ItemID.XenonMoss;

        public float Divisor = 1.5f;

        public override bool CanRoll(Item item) {
            return base.CanRoll(item);
        }

        public override bool CanRollArmorPrefix(Item item) {
            return base.CanRollArmorPrefix(item) && item.defense > Divisor;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            return new[] {
                new TooltipLine(Mod, "NeonPrefixEffect", $"75% of your defense is converted into endurance")
                { IsModifier = true, IsModifierBad = false, },

                new TooltipLine(Mod, "NeonPrefixEffect2", $"+{GetEnduranceAmount(item)}% Damage Reduction")
                { IsModifier = true, IsModifierBad = false, }
            };
        }

        public int GetEnduranceAmount(Item item) {
            return (int)Math.Round(item.defense / Divisor);
        }

        public override void Apply(Item item) {
            item.Aequus().defenseChange = -item.defense;
        }

        public override void UpdateEquip(Item item, Player player) {
            player.endurance += GetEnduranceAmount(item) / 100f;
        }
    }
}