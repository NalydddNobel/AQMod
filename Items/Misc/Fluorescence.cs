using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Fluorescence : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = RaritySets.RarityGaleStreams;
            Item.value = Item.sellPrice(silver: 15);
        }
    }
}