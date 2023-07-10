using Terraria;
using Terraria.ID;

namespace Aequus.Items {
    public partial class AequusItem {
        private void CheckResonance(Item item) {
            if (item.type == ItemID.HelFire && Main.remixWorld) {
                NameTag = "Resonance";
            }
        }
    }
}