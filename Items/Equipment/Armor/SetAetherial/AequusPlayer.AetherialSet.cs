using Aequus.Items.Equipment.Armor.SetAetherial;
using Terraria;

namespace Aequus {
    public partial class AequusPlayer {
        public bool armorAetherialAmmoCost;

        private bool CanConsume_AetherialSet() {
            return armorAetherialAmmoCost && Main.rand.NextFloat() > AetherialCrown.NoConsumeChance;
        }
    }
}