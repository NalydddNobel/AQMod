using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Buildings
{
    public class PirateBountyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 8;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}