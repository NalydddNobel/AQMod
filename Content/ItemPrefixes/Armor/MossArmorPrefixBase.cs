using Terraria;

namespace Aequus.Content.ItemPrefixes.Armor {
    public abstract class MossArmorPrefixBase : AequusPrefix {
        public override bool Shimmerable => true;

        public abstract int MossItem { get; }

        public override bool CanRoll(Item item) {
            return false;
        }

        public virtual bool CanRollArmorPrefix(Item item) {
            return item.IsArmor();
        }

        public override void ModifyValue(ref float valueMult) {
            valueMult = 1.1f;
        }
    }
}