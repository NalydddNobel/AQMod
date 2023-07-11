using Terraria;
using Terraria.ID;

namespace Aequus.Items {
    public partial class AequusItem {
        private void CheckResonance(Item item) {
            if (item.type == ItemID.HelFire && Aequus.ZenithSeed) {
                NameTag = "Resonance";
                CheckNameTag(item);
            }
        }
    }
}