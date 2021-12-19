using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class SiphonTentacle : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.rare = AQItem.Rarities.GaleStreamsRare;
            item.value = Item.sellPrice(silver: 15);
        }
    }
}