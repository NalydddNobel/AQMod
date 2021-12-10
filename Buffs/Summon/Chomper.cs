using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Summon
{
    public class Chomper : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.ChomperMinion.Chomper>()] > 0)
            {
                aQPlayer.chomper = true;
            }
            if (!aQPlayer.chomper)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}