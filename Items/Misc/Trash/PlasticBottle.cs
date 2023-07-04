using Aequus.Common.DataSets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Trash {
    public class PlasticBottle : ModItem {
        public override void SetStaticDefaults() {
            ItemSets.FishingTrashForDevilsTounge.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 10;
        }
    }
}