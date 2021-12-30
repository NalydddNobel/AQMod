using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Timers
{
    public abstract class TimerActiveBuff : ModBuff
    {
        public abstract int GetTick(Player player);

        public override void Update(Player player, ref int buffIndex)
        {
            int timer = GetTick(player);
            if (timer <= 0)
            {
                return;
            }
            player.buffTime[buffIndex] = timer + 2;
        }
    }
}