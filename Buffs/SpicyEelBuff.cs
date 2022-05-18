using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class SpicyEelBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsWellFed[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Aequus().buffSpicyEel = true;
            player.jumpSpeedBoost *= 1.1f;
            player.wingAccRunSpeed *= 1.1f;
            player.moveSpeed += 1f;
            player.moveSpeed *= 1.1f;
            player.luck += 0.05f;
        }
    }
}