using Aequus.Content.Carpentery.Photobook;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Tools.Camera
{
    public class PeonyPhotobook : Photobook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            player.GetModPlayer<PhotobookPlayer>().UpgradePhotos(32);
        }

        public override void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            base.UpdateBank(player, aequus, slot, bank);
            player.GetModPlayer<PhotobookPlayer>().UpgradePhotos(32);
        }
    }
}