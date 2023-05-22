using Aequus.Items.Accessories.CrownOfBlood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc.Money {
    public class RichMansMonocle : ModItem, ItemHooks.IUpdateVoidBag {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
        }

        public override void UpdateInventory(Player player) {
            player.Aequus().accPriceMonocle = true;
        }

        public void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank) {
            aequus.accPriceMonocle = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accPriceMonocle = true;
        }
    }
}