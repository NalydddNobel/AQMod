using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools
{
    public class RichMansMonocle : ModItem, Hooks.IUpdateBank
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