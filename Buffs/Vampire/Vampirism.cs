using AQMod.Buffs.Timers;
using AQMod.Content.Players;
using Terraria;

namespace AQMod.Buffs.Vampire
{
    public class Vampirism : TimerActiveBuff
    {
        public override int GetTick(Player player)
        {
            return player.GetModPlayer<VampirismPlayer>().Vampirism;
        }

        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<VampirismPlayer>().IsVampire)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            base.Update(player, ref buffIndex);
        }
    }
}