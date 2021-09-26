using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class CrabShell : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = Item.sellPrice(silver: 8);
            item.maxStack = 999;
        }
    }
}