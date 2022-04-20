using Aequus.Common.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Fluorescence : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6));
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemMaterialSortingConstants.SoulOfFlight;
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            this.SetResearch(25);
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = ItemRarities.GaleStreams - 1;
            Item.value = Item.sellPrice(silver: 15);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}