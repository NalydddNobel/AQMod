using Aequus.Tiles;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Trash
{
    public class PlasticBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            RecyclingTable.Convert.Add(Type, new List<RecyclingTable.Info>()
            {
                ItemID.Bottle,
                ItemID.Mug,
                ItemID.ClayPot,
            });

            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 10;
        }
    }
}