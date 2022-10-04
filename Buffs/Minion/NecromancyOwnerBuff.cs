using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
            if (Main.LocalPlayer.buffTime[buffIndex] > 2)
            {
                Main.LocalPlayer.buffTime[buffIndex] = 2;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] > 2)
                player.buffTime[buffIndex] = 30;
        }
    }
}