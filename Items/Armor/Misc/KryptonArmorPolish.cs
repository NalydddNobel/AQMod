using Aequus.Content.ItemPrefixes.Armor;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Armor.Misc {
    public class KryptonArmorPolish : ArmorPolishItem<KryptonPrefix> {
        public override string Texture => AequusTextures.ElitePlantKrypton.Path;

        public override void SetDefaults() {
            base.SetDefaults();
            Item.color = Colors.CoinCopper;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }
    }
}