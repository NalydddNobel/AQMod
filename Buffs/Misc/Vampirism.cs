using Aequus.Common.ModPlayers;
using Terraria;

namespace Aequus.Buffs.Misc
{
    public class Vampirism : BaseSpecialTimerBuff
    {
        public override int GetTick(Player player)
        {
            return player.GetModPlayer<PlayerVampirism>().vampirism;
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.ConcoctibleBuffIDsBlacklist.Add(Type);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<PlayerVampirism>().IsVampire)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            base.Update(player, ref buffIndex);
        }
    }
}