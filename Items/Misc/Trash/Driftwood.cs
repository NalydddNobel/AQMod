using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Trash {
    public class Driftwood : ModItem {
        public override void SetStaticDefaults() {
            AequusPlayer.TrashItemIDs.Add(Type);

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 10;
        }
    }
}