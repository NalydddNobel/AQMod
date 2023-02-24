using Aequus.Events.DemonSiege;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Events.DemonSiege.Misc
{
    public class UnholyCoreSmall : ModItem
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
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}