using Aequus.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class DoubledTimePrefix : ModPrefix
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
            return ConcoctionDatabase.ConcoctiblePotion(item);
        }
    }
}
