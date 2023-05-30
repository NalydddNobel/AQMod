using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs {
    public class SpicyEelBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsFedState[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.jumpSpeedBoost += 1f;
            player.wingAccRunSpeed *= 1.1f;
            player.moveSpeed += 0.6f;
            player.luck += 0.03f;

            var aequus = player.Aequus();
            aequus.wingStats.horizontalSpeed += 0.3f;
            aequus.wingStats.verticalMaxCanAscendMultiplier += 0.02f;
            aequus.wingStats.verticalMaxAscentMultiplier += 0.05f;
        }
    }
}