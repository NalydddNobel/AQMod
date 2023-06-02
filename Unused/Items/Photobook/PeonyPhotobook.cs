using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Aequus.Unused.Items.Photobook {
    public class PeonyPhotobook : PhotobookItem {
        public new const int PhotoStorage = 32;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PhotoStorage);

        public override void SetDefaults() {
            base.SetDefaults();
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.buyPrice(gold: 5);
        }
    }
}