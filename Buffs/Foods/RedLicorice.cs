using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Foods
{
    public class RedLicorice : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.wellFed = true;
            player.statDefense += 3;
            player.allDamage += 0.075f;
            player.meleeSpeed += 0.075f;

            player.rangedCrit += 3;
            player.magicCrit += 3;
            player.thrownCrit += 3;

            player.minionKB += 0.75f;
            player.moveSpeed += 0.3f;

            player.meleeDamage += 0.025f;
            player.meleeCrit += 4;
            player.statLifeMax2 += 20;
        }
    }
}