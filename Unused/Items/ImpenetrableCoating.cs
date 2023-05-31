using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [LegacyName("TitaniumPaintbrush", "TitaniumScraper")]
    public class ImpenetrableCoating : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.RedPaint);
            Item.paint = 0;
            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Gray;
        }
    }
}