using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods.SpicyEel {
    public class SpicyEelBuff : ModBuff {
        public override void SetStaticDefaults() {
            BuffID.Sets.IsFedState[Type] = true;
            BuffID.Sets.IsWellFed[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.jumpSpeedBoost += 1f;
            player.wingAccRunSpeed *= 1.1f;
            player.moveSpeed += 0.6f;
            player.luck += 0.03f;

            var aequus = player.Aequus();
            aequus.flightStats.horizontalSpeed += 0.3f;
            aequus.flightStats.verticalMaxCanAscendMultiplier += 0.02f;
            aequus.flightStats.verticalMaxAscentMultiplier += 0.05f;
        }
    }
}