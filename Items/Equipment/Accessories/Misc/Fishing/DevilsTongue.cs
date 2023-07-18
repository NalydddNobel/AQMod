using Aequus.Common.Items.EquipmentBooster;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Misc.Fishing {
    public class DevilsTongue : ModItem {
        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetNoEffect(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accDevilsTongue = true;
        }
    }
}