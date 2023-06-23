using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Necromancy {
    public abstract class NecromancyPrefixBase : ModPrefix {
        public override PrefixCategory Category => PrefixCategory.Magic;

        public override bool CanRoll(Item item) {
            return item.DamageType.CountsAsClass(Aequus.NecromancyClass);
        }
    }
}