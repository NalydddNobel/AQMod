using Aequus.Content.ItemPrefixes.Necromancy;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreBase : ModItem {
        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
        }

        public override void SetDefaults() {
            Item.DamageType = Aequus.NecromancyMagicClass;
        }

        public override bool AllowPrefix(int pre) {
            return PrefixLoader.GetPrefix(pre) is NecromancyPrefixBase;
        }
    }
}