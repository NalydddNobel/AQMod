using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class BaguetteBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.wellFed = true;
            player.lifeRegen += 4;
            player.statDefense += 10;
            player.allDamage += 0.1f;
            player.meleeSpeed += 0.1f;

            player.meleeCrit += 10;
            player.rangedCrit += 10;
            player.magicCrit += 10;
            player.thrownCrit += 10;

            player.minionKB += 1f;
            player.moveSpeed += 1f;
        }
    }
}