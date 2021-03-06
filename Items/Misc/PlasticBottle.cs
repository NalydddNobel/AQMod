using Aequus.Common;
using Aequus.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class PlasticBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            RecyclingTable.Convert.Add(Type, new List<RecyclingTable.Info>()
            {
                new RecyclingTable.Info(ItemID.Bottle, 1, 4),
                new RecyclingTable.Info(ItemID.Mug, 1, 4),
                new RecyclingTable.Info(ItemID.ClayPot, 1, 3),
            });

            PlayerFishing.TrashItemIDs.Add(Type);

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 10;
        }
    }
}