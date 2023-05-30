using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Potions {
    public class BoundedPrefix : PotionPrefixBase {
        public override bool Shimmerable => true;
        public override string GlintTexture => $"{this.NamespacePath()}/BoundedGlint";

        public override void Apply(Item item) {
            item.Aequus().prefixPotionsBounded = true;
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 1.1f;
        }

        public override bool CanRoll(Item item) {
            return AequusItem.IsPotion(item) && !Main.persistentBuff[item.buffType];
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item) {
            return new[] { new TooltipLine(Aequus.Instance, "PrefixBounded", TextHelper.GetTextValue("Prefixes.BoundedPrefix.Ability"))
            { IsModifier = true, IsModifierBad = false, } };
        }
    }
}