using AQMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Minion
{
    public class StariteMinion : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.StariteMinionLeader>()] > 0)
            {
                aQPlayer.stariteMinion = true;
            }
            if (!aQPlayer.stariteMinion)
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