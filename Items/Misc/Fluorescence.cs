using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Fluorescence : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6));
            this.SetResearch(25);
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = ItemRarities.GaleStreams;
            Item.value = Item.sellPrice(silver: 15);
        }
    }
}