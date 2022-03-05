using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Summon
{
    public class SnowsawBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.SnowsawMinion>()] > 0)
            {
                aQPlayer.snowsaw = true;
            }
            if (!aQPlayer.snowsaw)
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