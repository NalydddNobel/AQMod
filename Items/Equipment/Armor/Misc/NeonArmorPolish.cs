using Aequus.Content.ItemPrefixes.Armor;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Equipment.Armor.Misc {
    public class NeonArmorPolish : ArmorPolishItem<NeonPrefix> {
        public override string Texture => AequusTextures.ElitePlantNeon.FullPath;

        public override void SetDefaults() {
            base.SetDefaults();
            Item.color = Colors.CoinCopper;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }
    }
}