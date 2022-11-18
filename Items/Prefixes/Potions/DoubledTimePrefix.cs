using Aequus.Buffs;
using Aequus.Items.GlobalItems;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class DoubledTimePrefix : AequusPrefix
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Apply(Item item)
        {
            item.buffTime *= 2;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item)
        {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.buffNoTimeDisplay[item.buffType] && !Main.meleeBuff[item.buffType] &&
                !AequusBuff.ConcoctibleBuffIDsBlacklist.Contains(item.buffType);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.buffTime != ContentSamples.ItemsByType[item.type].buffTime)
            {
                TooltipsGlobal.PercentageModifier(item.buffTime, ContentSamples.ItemsByType[item.type].buffTime, "BuffDuration", tooltips, higherIsGood: true);
            }
        }
    }
}