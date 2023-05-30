using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes {
    public abstract class AequusPrefix : ModPrefix
    {
        public virtual bool Shimmerable => false;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public virtual void UpdateEquip(Item item, Player player)
        {
        }

        public virtual void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
        }
    }
}