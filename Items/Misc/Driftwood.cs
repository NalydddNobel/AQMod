using Aequus;
using Aequus.NPCs.ExporterNPC.Shop;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Driftwood : ModItem
    {
        public override void SetStaticDefaults()
        {
            RecyclingTable.Convert.Add(Type, new List<RecyclingTable.Info>()
            {
                WoodInfo(ItemID.Wood),
                WoodInfo(ItemID.Ebonwood),
                WoodInfo(ItemID.Shadewood),
                WoodInfo(ItemID.RichMahogany),
                WoodInfo(ItemID.BorealWood),
                WoodInfo(ItemID.PalmWood),
                WoodInfo(ItemID.DynastyWood),
                WoodInfo(ItemID.Pearlwood, () => Main.hardMode),
            });

            AequusPlayer.TrashItemIDs.Add(Type);

            SacrificeTotal = 1;
        }

        private RecyclingTable.Info WoodInfo(int item, Func<bool> canObtain = null)
        {
            return new RecyclingTable.Info(item, 12, 64, canObtain);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 10;
        }
    }
}