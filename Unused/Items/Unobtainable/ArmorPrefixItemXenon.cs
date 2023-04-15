using Aequus.Content.ItemPrefixes.Armor;
using Aequus.Items.Armor.Misc;
using Terraria;
using Terraria.ID;

namespace Aequus.Unused.Items.Unobtainable {
    public class ArmorPrefixItemXenon : ArmorPrefixItem<XenonPrefix> {

        public override string Texture => AequusTextures.ElitePlantXenon.Path;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            // Tooltip.SetDefault("""
            //Applys the xenon prefix to armors
            //Right click on an armor while holding this to apply the prefix
            //Work in progress.
            //""");
        }

        public override void SetDefaults() {
            base.SetDefaults();
            Item.color = Colors.CoinCopper;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }
    }
}