using Aequus.Biomes.DemonSiege;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Summons
{
    public class UnholyCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(Type, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}