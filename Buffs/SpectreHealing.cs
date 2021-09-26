using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class SpectreHealing : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] += time;
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] % 5 == 0)
            {
                player.HealEffect(1);
                player.statLife += 1;
            }
        }
    }
}