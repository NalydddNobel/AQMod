using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Summon
{
    public class Monoxider : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.Monoxider>()] > 0)
            {
                aQPlayer.monoxiderBird = true;
            }
            if (!aQPlayer.monoxiderBird)
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
