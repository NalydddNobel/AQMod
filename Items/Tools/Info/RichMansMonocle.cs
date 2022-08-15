using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Info
{
    public class RichMansMonocle : ModItem, ItemHooks.IUpdateBank
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
            Item.accessory = false;
        }

        public override void UpdateInventory(Player player)
        {
            player.Aequus().showPrices = true;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            aequus.showPrices = true;
        }
    }
}