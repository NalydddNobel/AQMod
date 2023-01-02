using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class CrystalDaggerBuff : ModBuff
    {
        public override bool RightClick(int buffIndex)
        {
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.1f;
            player.runAcceleration += 0.1f;
        }
    }
}