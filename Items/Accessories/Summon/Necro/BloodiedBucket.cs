using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public sealed class BloodiedBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDemoniteCrimtane + 1;
            Item.value = ItemDefaults.CorruptionValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ghostLifespan *= 3;
        }
    }
}