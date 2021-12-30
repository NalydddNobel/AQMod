using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.NobleMushrooms
{
    public class ArgonMushroom : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(silver: 2);
            item.maxStack = 999;
        }
    }
}