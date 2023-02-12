using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Armor
{
    public class NeonPrefix : MossArmorPrefixBase
    {
        public override int MossItem => ItemID.XenonMoss;

        public float Divisor = 1.5f;

        public override bool CanRoll(Item item)
        {
            return base.CanRoll(item) && item.defense > Divisor;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            AddPrefixLine(tooltips, new TooltipLine(Mod, "KryptonPrefixEffect", $"75% of your defense is converted into endurance") { IsModifier = true, IsModifierBad = false, });
            AddPrefixLine(tooltips, new TooltipLine(Mod, "KryptonPrefixEffect", $"+{GetEnduranceAmount(item)}% Damage Reduction") { IsModifier = true, IsModifierBad = false, });
            //AddPrefixLine(tooltips, new TooltipLine(Mod, "KryptonPrefixEffect", "Defense is removed from armor") { IsModifier = true, IsModifierBad = true, });
        }

        public int GetEnduranceAmount(Item item)
        {
            return (int)Math.Round(item.defense / Divisor);
        }

        public override void Apply(Item item)
        {
            item.Aequus().defenseChange = -item.defense;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.endurance += GetEnduranceAmount(item) / 100f;
        }
    }
}