using Aequus.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class TatteredDemonHorn : ModItem
    {
        public override void SetStaticDefaults()
        {
            RecyclingTable.Convert.Add(Type, new List<RecyclingTable.Info>()
            {
                new RecyclingTable.Info(ItemID.Obsidian, 3, 7),
                new RecyclingTable.Info(ItemID.LivingFireBlock, 1, 4, () => Main.hardMode),
                new RecyclingTable.Info(ItemID.IronBar, 1, 4),
                new RecyclingTable.Info(ItemID.LeadBar, 1, 4),
                new RecyclingTable.Info(ItemID.Cobweb, 40, 75),
                new RecyclingTable.Info(ItemID.EbonsandBlock, 6, 25),
                new RecyclingTable.Info(ItemID.CrimsandBlock, 6, 25),
                new RecyclingTable.Info(ItemID.HellfireArrow, 40, 120),
                new RecyclingTable.Info(ItemID.SiltBlock, 20, 100),
                new RecyclingTable.Info(ItemID.AshBlock, 20, 100),
                ItemID.ObsidianSkinPotion,
            });

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 16;
        }
    }
}
