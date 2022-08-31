using Aequus.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Builder
{
    public class OmniPaint : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.RedPaint);
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5);
            Item.consumable = false;
            Item.paint = 0;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.myPlayer == player.whoAmI && !Main.playerInventory)
            {
                OmniPaintInterface.Instance.Enabled = true;
            }
            Item.paint = player.Aequus().omniPaint;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }
    }
}