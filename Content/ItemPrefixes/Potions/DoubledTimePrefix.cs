using Aequus.Common.Buffs;
using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Potions {
    public class DoubledTimePrefix : PotionPrefixBase {
        public override bool Shimmerable => true;
        public override string GlintTexture => $"{this.NamespacePath()}/StuffedGlint";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Apply(Item item) {
            item.buffTime *= 2;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item) {
            return item.buffType > 0 && item.buffTime > 0 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && !Main.buffNoTimeDisplay[item.buffType] && !Main.meleeBuff[item.buffType] &&
                !AequusBuff.ConcoctibleBuffIDsBlacklist.Contains(item.buffType);
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            var defaultItem = ContentSamples.ItemsByType[item.type];
            if (item.buffTime == defaultItem.buffTime) {
                return null;
            }

            var tt = AequusItem.PercentageModifierLine(item.buffTime, defaultItem.buffTime, "DoubledTimePrefix.Ability");
            return new[] { tt };
        }
    }
}