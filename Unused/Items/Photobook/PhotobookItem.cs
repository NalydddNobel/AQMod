using Aequus.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.Photobook {
    [LegacyName("Photobook")]
    public class PhotobookItem : ModItem {
        public const int PhotoStorage = 20;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(PhotoStorage);

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = AequusSounds.photobookopen;
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}