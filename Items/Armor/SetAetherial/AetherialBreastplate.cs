using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.SetAetherial {
    [AutoloadEquip(EquipType.Body)]
    public class AetherialBreastplate : ModItem {
        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.defense = 0;
            Item.rare = ItemRarityID.Red;
        }
    }
}