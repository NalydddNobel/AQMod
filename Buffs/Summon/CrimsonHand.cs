using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Summon
{
    public class CrimsonHand : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.CrimsonHand>()] > 0)
            {
                aQPlayer.crimsonHands = true;
            }
            if (!aQPlayer.crimsonHands)
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
