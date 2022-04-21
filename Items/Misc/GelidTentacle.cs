using Aequus.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class GelidTentacle : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemMaterialSortingConstants.SoulOfFlight;
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
    }
}