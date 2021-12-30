using AQMod.Buffs.Delays;
using AQMod.Content.Players;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Timers
{
    public class Vampirism : TimerActiveBuff
    {
        public override int GetTick(Player player)
        {
            return player.GetModPlayer<VampirismPlayer>().Vampirism;
        }

        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().bossrush = true;
            player.aggro += 1000;
        }
    }
}