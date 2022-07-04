using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class NecromancyOwnerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.buffTime[buffIndex] = Math.Min(Main.LocalPlayer.buffTime[buffIndex], 2);
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] > 2)
                player.buffTime[buffIndex] = 30;
        }
    }
}