using Aequus.Content.ItemPrefixes.Armor;
using Aequus.Items.Armor.Misc;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Unused.Unobtainable {
    public class ArmorPrefixItemArgon : ArmorPrefixItem<ArgonPrefix> {

        public override string Texture => AequusTextures.ElitePlantArgon.Path;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            // Tooltip.SetDefault("""
            //Applys the argon prefix to armors
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