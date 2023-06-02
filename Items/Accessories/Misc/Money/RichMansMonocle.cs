using Aequus.Items.Accessories.CrownOfBlood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc.Money {
    public class RichMansMonocle : ModItem {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
        }

        public override void UpdateInfoAccessory(Player player) {
            player.Aequus().accPriceMonocle = true;
        }
    }
}