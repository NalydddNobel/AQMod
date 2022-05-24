using Aequus.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Trash
{
    public class Driftwood : ModItem
    {
        public override void SetStaticDefaults()
        {
            RecyclingTable.Convert.Add(Type, new List<RecyclingTable.Info>()
            {
                ItemID.Wood,
                ItemID.Ebonwood,
                ItemID.Shadewood,
                ItemID.RichMahogany,
                ItemID.BorealWood,
                ItemID.PalmWood,
                ItemID.DynastyWood,
                new RecyclingTable.Info(ItemID.Pearlwood, () => Main.hardMode),
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